using ProyectoAeroline;                  // LoginViewModel
using ProyectoAeroline.Data;             // LoginData, UsuariosData (lo resolveremos con GetService)
using ProyectoAeroline.Models;           // RegisterViewModel, ForgotPasswordViewModel, ResetPasswordViewModel
using ProyectoAeroline.Services;         // IEmailService
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ProyectoAeroline.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly LoginData _loginData;
        private readonly IEmailService _email;   // NUEVO: servicio de correo

        public AccountController(LoginData loginData, IEmailService email)
        {
            _loginData = loginData;
            _email = email;
        }

        // ================== LOGIN (GET) ==================
        [HttpGet("Login")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (User?.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session?.Clear();
                // NO borres "todas" las cookies aquí
            }

            // 🔹 borra posible cookie antiforgery viejo con nombre anterior
            Response.Cookies.Delete("Aero.AntiForgery");

            // 🔹 emite el antiforgery cookie/tokens para este primer render
            var af = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
            af.GetAndStoreTokens(HttpContext);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // ================== LOGIN (POST) ==================
        [HttpPost("Login")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            // Validación real contra tu SP
            var user = await _loginData.ValidarUsuarioAsync(model.Email, model.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Datos incorrectos.");
                ViewData["LoginError"] = "Datos incorrectos";
                return View(model);
            }

            // ★ Normalizar datos de usuario
            var nombre = (user.Nombre ?? model.Email)?.Trim() ?? model.Email;
            var correo = (user.Correo ?? model.Email)?.Trim() ?? model.Email;
            var rolNombre = string.IsNullOrWhiteSpace(user.NombreRol) ? "Usuario" : user.NombreRol.Trim();

            // ★ Claims (incluyo ClaimTypes.Name y un claim "Nombre" por comodidad)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, nombre),
                new Claim("Nombre", nombre),
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Role, rolNombre)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ★ Propiedades de autenticación (persistencia y expiración opcional)
            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) // opcional
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            // ★ Session (si la usas en otras partes)
            HttpContext.Session.SetInt32("IdUsuario", user.IdUsuario);
            HttpContext.Session.SetString("Nombre", nombre);
            HttpContext.Session.SetString("Correo", correo);
            HttpContext.Session.SetString("Rol", rolNombre);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return LocalRedirect("/Index");
        }

        // ================== LOGOUT (GET) ==================
        [HttpGet("Logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var c in Request.Cookies.Keys) Response.Cookies.Delete(c);
            HttpContext.Session?.Clear();

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Login");
        }

        // ================== LOGOUT (POST) ==================
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            foreach (var c in Request.Cookies.Keys) Response.Cookies.Delete(c);

            return RedirectToAction("Login");
        }

        // ===================================================
        // =============== NUEVO: GOOGLE LOGIN ===============
        // ===================================================

        // Inicia el challenge con Google
        [HttpGet("ExternalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider = "Google", string? returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var props = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(props, provider); // "Google"
        }

        // Callback de Google (vincula/crea usuario y firma cookie local)
        [HttpGet("ExternalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = "/")
        {
            // Claims del proveedor
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            var provider = "Google";
            var providerKey = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // sub

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
            {
                TempData["LoginError"] = "No se pudo obtener información de Google.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // Resolver UsuariosData solo cuando lo necesitemos (no rompe DI anterior)
            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                TempData["LoginError"] = "Servicio de usuarios no disponible.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // 1) ¿Existe mapping externo?
            var usuario = await usuarios.BuscarPorExternalLoginAsync(provider, providerKey);

            // 2) Si no existe: intenta por email; si tampoco existe, crea uno “externo”
            if (usuario == null)
            {
                usuario = await usuarios.BuscarPorCorreoAsync(email);
                if (usuario == null)
                {
                    // Crear usuario básico (Estado=Activo, Rol por defecto adentro del SP)
                    var nuevoId = await usuarios.CrearUsuarioBasicoAsync(nombre: (User.Identity?.Name ?? email).Split('@')[0],
                                                                        correo: email,
                                                                        rolId: 2,
                                                                        estado: "Activo");
                    if (nuevoId <= 0)
                    {
                        TempData["LoginError"] = "No se pudo crear el usuario.";
                        return RedirectToAction("Login", new { returnUrl });
                    }

                    usuario = await usuarios.BuscarPorCorreoAsync(email);
                    if (usuario == null)
                    {
                        TempData["LoginError"] = "No se pudo obtener el usuario recién creado.";
                        return RedirectToAction("Login", new { returnUrl });
                    }
                }

                // 3) Guarda mapping proveedor→usuario
                await usuarios.GuardarExternalLoginAsync(usuario.IdUsuario, provider, providerKey,
                                                         emailProveedor: email,
                                                         displayName: User.Identity?.Name ?? usuario.Nombre,
                                                         avatarUrl: null);
            }

            // 4) Firmar cookie local
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? email),
                new Claim("Nombre", usuario.Nombre ?? email),
                new Claim(ClaimTypes.Email, usuario.Correo ?? email),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(usuario.NombreRol) ? "Usuario" : usuario.NombreRol!)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true, // puedes enlazarlo a RememberMe si lo deseas
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            // (Opcional) auditoría de acceso
            await usuarios.RegistrarAccesoExternoAsync(provider, providerKey, HttpContext.Connection.RemoteIpAddress?.ToString());

            return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }

        // ===================================================
        // =============== NUEVO: REGISTRO ===================
        // ===================================================

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                ModelState.AddModelError("", "Servicio de usuarios no disponible.");
                return View(vm);
            }

            // Valida duplicado por correo
            var existente = await usuarios.BuscarPorCorreoAsync(vm.Correo);
            if (existente != null)
            {
                ModelState.AddModelError(nameof(vm.Correo), "El correo ya está registrado.");
                return View(vm);
            }

            // (Recomendado) hashear antes de guardar: aquí podrías aplicar tu PasswordHasher si lo usas
            // var hashed = PasswordHasher.Hash(vm.Password);

            var id = await usuarios.CrearUsuarioBasicoAsync(vm.Nombre, vm.Correo, rolId: 2, estado: "Activo");
            if (id <= 0)
            {
                ModelState.AddModelError("", "No se pudo crear la cuenta.");
                return View(vm);
            }

            // (Opcional) enviar verificación de correo
            // var token = await usuarios.CrearTokenVerificacionAsync(id);
            // await _email.SendAsync(vm.Correo, "Verifique su correo", templateHtmlConLink);

            TempData["Info"] = "Cuenta creada. Ya puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // ======== NUEVO: OLVIDÉ MI CONTRASEÑA ==============
        // ===================================================

        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                ModelState.AddModelError("", "Servicio de usuarios no disponible.");
                return View(vm);
            }

            // Generar token (la SP debe devolver token aunque el correo no exista para no revelar)
            var (token, idUsuario) = await usuarios.CrearTokenResetAsync(vm.Correo,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers.UserAgent.ToString());

            // Componer link y plantilla
            var link = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
                         "Views", "Shared", "Emails", "ResetPassword.html"))
                        .Replace("{{link}}", link ?? "#")
                        .Replace("{{nombre}}", vm.Correo.Split('@')[0])
                        .Replace("{{minutos}}", "30");

            // Enviar correo (si SMTP está configurado)
            try { await _email.SendAsync(vm.Correo, "Restablecer contraseña", html); }
            catch { /* No reveles error de envío al usuario final */ }

            TempData["Info"] = "Si el correo existe, recibirá un enlace para restablecer su contraseña.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // =============== NUEVO: RESET PASSWORD =============
        // ===================================================

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                ModelState.AddModelError("", "Servicio de usuarios no disponible.");
                return View(vm);
            }

            // (Recomendado) hashear aquí antes de guardar, si usas hash local
            // var hashed = PasswordHasher.Hash(vm.NuevaPassword);

            var (ok, code) = await usuarios.ConsumirTokenResetAsync(vm.Token, vm.NuevaPassword);
            if (!ok)
            {
                ModelState.AddModelError("", code == "TOKEN_INVALIDO" ? "Token inválido o expirado." : "No fue posible restablecer la contraseña.");
                return View(vm);
            }

            TempData["Info"] = "Contraseña actualizada. Inicie sesión con su nueva contraseña.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // ======== (OPCIONAL) CONFIRMAR EMAIL ===============
        // ===================================================

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                ViewBag.Success = false;
                ViewBag.Message = "Servicio de usuarios no disponible.";
                return View();
            }

            var (ok, code) = await usuarios.ConfirmarVerificacionAsync(token);
            ViewBag.Success = ok;
            ViewBag.Message = ok ? "Correo verificado correctamente." : "Token inválido o expirado.";
            return View();
        }
    }
}
