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
using Microsoft.Data.SqlClient;

namespace ProyectoAeroline.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        private readonly LoginData _loginData;
        private readonly IEmailService _email;
        private readonly UsuariosData _usuariosData;

        public AccountController(LoginData loginData, IEmailService email, UsuariosData usuariosData)
        {
            _loginData = loginData;
            _email = email;
            _usuariosData = usuariosData;
        }

        // ================== LOGIN (GET) ==================
        [HttpGet("Login")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login(string? returnUrl = null, string? error = null, string? message = null)
        {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (User?.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync(AuthScheme);
                HttpContext.Session?.Clear();
            }

            // Antiforgery fresco
            Response.Cookies.Delete("Aero.AntiForgery");
            var af = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
            af.GetAndStoreTokens(HttpContext);

            // Mostrar mensaje de error si viene de OAuth
            if (!string.IsNullOrWhiteSpace(error))
            {
                if (error == "correlation_failed")
                {
                    ViewData["LoginError"] = !string.IsNullOrWhiteSpace(message) ? message : "La sesión de autenticación expiró. Por favor, intenta iniciar sesión con Google nuevamente.";
                }
                else if (error == "access_denied")
                {
                    ViewData["LoginError"] = "Acceso denegado. No se pudo completar la autenticación con Google.";
                }
                else if (error == "oauth_failed")
                {
                    ViewData["LoginError"] = !string.IsNullOrWhiteSpace(message) ? message : "Error al autenticar con Google. Por favor, intenta nuevamente.";
                }
            }

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

            var user = await _loginData.ValidarUsuarioAsync(model.Email, model.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Datos incorrectos.");
                ViewData["LoginError"] = "Datos incorrectos";
                return View(model);
            }

            var nombre = (user.Nombre ?? model.Email)?.Trim() ?? model.Email;
            var correo = (user.Correo ?? model.Email)?.Trim() ?? model.Email;
            var rolNombre = string.IsNullOrWhiteSpace(user.NombreRol) ? "Usuario" : user.NombreRol.Trim();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, nombre),
                new Claim("Nombre", nombre),
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Role, rolNombre),
                new Claim("IdRol", user.IdRol.ToString()) // Agregar IdRol para verificación de permisos
            };

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(AuthScheme, principal, authProps);

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
            await HttpContext.SignOutAsync(AuthScheme);
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
            await HttpContext.SignOutAsync(AuthScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ===================================================
        // =============== GOOGLE LOGIN ======================
        // ===================================================

        [HttpGet("ExternalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider = "Google", string? returnUrl = "/")
        {
            // Limpiar cualquier sesión previa antes de iniciar OAuth
            HttpContext.Session?.Clear();
            Response.Cookies.Delete(".Aeroline.Auth");
            
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var props = new AuthenticationProperties 
            { 
                RedirectUri = redirectUrl,
                // Agregar propiedades para mejorar la seguridad del estado OAuth
                IsPersistent = false,
                AllowRefresh = false
            };
            return Challenge(props, provider); // "Google"
        }

        [HttpGet("ExternalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = "/", string? error = null, string? remoteError = null)
        {
            // Si hay un error explícito en la query string, redirigir al login
            if (!string.IsNullOrWhiteSpace(error) || !string.IsNullOrWhiteSpace(remoteError))
            {
                TempData["LoginError"] = "Error al autenticar con Google. Por favor, intenta nuevamente.";
                return RedirectToAction("Login", new { returnUrl });
            }
            
            // Si ya viene autenticado (porque Google firmó directo en el cookie), usa HttpContext.User
            ClaimsPrincipal? principal = HttpContext.User;

            // Respaldo: intenta leer la identidad del esquema de cookies por si el User aún no fue set
            if (principal?.Identity?.IsAuthenticated != true)
            {
                var auth = await HttpContext.AuthenticateAsync(AuthScheme);
                if (auth.Succeeded && auth.Principal != null)
                    principal = auth.Principal;
            }

            if (principal?.Identity?.IsAuthenticated != true)
            {
                TempData["LoginError"] = "No se pudo autenticar con Google.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // Claims del proveedor
            var provider = "Google";
            var providerKey = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? principal.FindFirst("sub")?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                     ?? principal.FindFirst("email")?.Value;
            var displayName = principal.Identity?.Name
                           ?? principal.FindFirst("name")?.Value
                           ?? email;

            if (string.IsNullOrWhiteSpace(providerKey) || string.IsNullOrWhiteSpace(email))
            {
                TempData["LoginError"] = "No se pudo obtener información del perfil de Google.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // Verificar si el usuario ya existe (ya tiene cuenta y está vinculado)
            var usuarioExistente = await _usuariosData.BuscarPorExternalLoginAsync(provider, providerKey);
            
            if (usuarioExistente != null)
            {
                // Usuario ya existe y está vinculado, permitir login directo
                var nombreFinal = usuarioExistente.Nombre ?? displayName ?? email;
                var correoFinal = usuarioExistente.Correo ?? email;
                var rolFinal = string.IsNullOrWhiteSpace(usuarioExistente.NombreRol) ? "Usuario" : usuarioExistente.NombreRol!;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioExistente.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, nombreFinal),
                    new Claim("Nombre", nombreFinal),
                    new Claim(ClaimTypes.Email, correoFinal),
                    new Claim(ClaimTypes.Role, rolFinal),
                    new Claim("IdRol", usuarioExistente.IdRol.ToString()) // Agregar IdRol para verificación de permisos
                };

                var identity = new ClaimsIdentity(claims, AuthScheme);
                await HttpContext.SignInAsync(
                    AuthScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) }
                );

                HttpContext.Session.SetInt32("IdUsuario", usuarioExistente.IdUsuario);
                HttpContext.Session.SetString("Nombre", nombreFinal);
                HttpContext.Session.SetString("Correo", correoFinal);
                HttpContext.Session.SetString("Rol", rolFinal);

                await _usuariosData.RegistrarAccesoExternoAsync(provider, providerKey, HttpContext.Connection.RemoteIpAddress?.ToString());

                if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
                    returnUrl = "/Index";

                // Agregar parámetro para indicar que viene de Google login y limpiar historial
                // Esto permite que el JavaScript en Index limpie el historial OAuth
                returnUrl = returnUrl.Contains("?") 
                    ? returnUrl + "&from=google" 
                    : returnUrl + "?from=google";
                
                return Redirect(returnUrl);
            }

            // Usuario no existe - verificar si ya tiene cuenta por correo (pero sin vincular Google)
            var usuarioPorCorreo = await _usuariosData.BuscarPorCorreoAsync(email);
            if (usuarioPorCorreo != null)
            {
                // Ya tiene cuenta local, solo vincular Google
                await _usuariosData.GuardarExternalLoginAsync(
                    usuarioPorCorreo.IdUsuario, provider, providerKey,
                    emailProveedor: email, displayName: displayName, avatarUrl: null
                );

                // Hacer login directo
                var nombreFinal = usuarioPorCorreo.Nombre ?? displayName ?? email;
                var correoFinal = usuarioPorCorreo.Correo ?? email;
                var rolFinal = string.IsNullOrWhiteSpace(usuarioPorCorreo.NombreRol) ? "Usuario" : usuarioPorCorreo.NombreRol!;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioPorCorreo.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, nombreFinal),
                    new Claim("Nombre", nombreFinal),
                    new Claim(ClaimTypes.Email, correoFinal),
                    new Claim(ClaimTypes.Role, rolFinal),
                    new Claim("IdRol", usuarioPorCorreo.IdRol.ToString()) // Agregar IdRol para verificación de permisos
                };

                var identity = new ClaimsIdentity(claims, AuthScheme);
                await HttpContext.SignInAsync(
                    AuthScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) }
                );

                HttpContext.Session.SetInt32("IdUsuario", usuarioPorCorreo.IdUsuario);
                HttpContext.Session.SetString("Nombre", nombreFinal);
                HttpContext.Session.SetString("Correo", correoFinal);
                HttpContext.Session.SetString("Rol", rolFinal);

                await _usuariosData.RegistrarAccesoExternoAsync(provider, providerKey, HttpContext.Connection.RemoteIpAddress?.ToString());

                if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
                    returnUrl = "/Index";

                // Agregar parámetro para indicar que viene de Google login y limpiar historial
                // Esto permite que el JavaScript en Index limpie el historial OAuth
                returnUrl = returnUrl.Contains("?") 
                    ? returnUrl + "&from=google" 
                    : returnUrl + "?from=google";
                
                return Redirect(returnUrl);
            }

            // Usuario completamente nuevo - crear token de confirmación y enviar email
            var token = await _usuariosData.CrearTokenConfirmacionGoogleAsync(
                providerKey,
                email,
                displayName ?? email.Split('@')[0],
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString()
            );

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["LoginError"] = "No se pudo generar el token de confirmación.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // Generar enlace de confirmación
            var confirmLink = Url.Action("ConfirmGoogleLogin", "Account", new { token }, Request.Scheme);

            // Cargar plantilla de email
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(),
                "Views", "Shared", "Emails", "GoogleLoginConfirmation.html");

            string emailHtml;
            if (System.IO.File.Exists(emailTemplatePath))
            {
                emailHtml = System.IO.File.ReadAllText(emailTemplatePath)
                    .Replace("{{link}}", confirmLink ?? "#")
                    .Replace("{{nombre}}", displayName ?? email.Split('@')[0])
                    .Replace("{{email}}", email);
            }
            else
            {
                // Fallback simple si no existe la plantilla
                emailHtml = $@"
                    <h2>Confirmar ingreso con Google</h2>
                    <p>Hola <strong>{displayName ?? email.Split('@')[0]}</strong>,</p>
                    <p>Has intentado iniciar sesión con Google usando el correo <strong>{email}</strong>.</p>
                    <p>Para completar el proceso y crear tu cuenta, haz clic en el siguiente enlace:</p>
                    <p><a href=""{confirmLink}"">Confirmar y crear cuenta</a></p>
                    <p>Este enlace expira en 60 minutos.</p>
                    <p>Si no realizaste esta solicitud, ignora este mensaje.</p>";
            }

            try
            {
                await _email.SendAsync(email, "Confirma tu ingreso con Google", emailHtml);
            }
            catch (Exception ex)
            {
                // Log error pero no revelar al usuario
                TempData["LoginError"] = "Error al enviar el correo de confirmación. Intenta nuevamente.";
                return RedirectToAction("Login", new { returnUrl });
            }

            // Mostrar mensaje de que se envió el email
            TempData["Info"] = $"Por favor verificar su correo electrónico. Se ha enviado un correo de confirmación a {email}. Revisa tu bandeja de entrada y haz clic en el enlace para completar tu registro.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // =============== REGISTRO LOCAL ====================
        // ===================================================

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // Limpiar cualquier mensaje de TempData anterior para evitar que aparezca en el formulario vacío
            if (TempData.ContainsKey("Info"))
            {
                TempData.Remove("Info");
            }
            return View(new RegisterViewModel());
        }

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

            var existente = await usuarios.BuscarPorCorreoAsync(vm.Correo);
            if (existente != null)
            {
                ModelState.AddModelError(nameof(vm.Correo), "El correo ya está registrado.");
                return View(vm);
            }

            // Hashear la contraseña antes de guardarla
            var passwordHash = ProyectoAeroline.Seguridad.PasswordHasher.Hash(vm.Password);
            
            // Obtener IdRol de Usuario dinámicamente
            var idRolUsuario = await ObtenerIdRolUsuarioAsync();
            var id = await usuarios.CrearUsuarioBasicoAsync(vm.Nombre, vm.Correo, rolId: idRolUsuario, estado: "Activo", passwordHash: passwordHash); // Rol Usuario por defecto
            if (id <= 0)
            {
                ModelState.AddModelError("", "No se pudo crear la cuenta.");
                return View(vm);
            }

            TempData["Info"] = "Cuenta creada. Ya puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // ============== OLVIDÉ MI CONTRASEÑA ===============
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

            var (token, idUsuario) = await usuarios.CrearTokenResetAsync(vm.Correo,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers.UserAgent.ToString());

            var link = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
                         "Views", "Shared", "Emails", "ResetPassword.html"))
                        .Replace("{{link}}", link ?? "#")
                        .Replace("{{nombre}}", vm.Correo.Split('@')[0])
                        .Replace("{{minutos}}", "30");

            try { await _email.SendAsync(vm.Correo, "Restablecer contraseña", html); }
            catch { /* Silenciar envío fallido para no revelar existencia de correo */ }

            TempData["Info"] = "Si el correo existe, recibirá un enlace para restablecer su contraseña.";
            return RedirectToAction("Login");
        }

        // ===================================================
        // ================== RESET PASSWORD =================
        // ===================================================

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string? fromGoogle = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            ViewBag.FromGoogle = !string.IsNullOrWhiteSpace(fromGoogle);
            if (ViewBag.FromGoogle == true)
            {
                ViewBag.Message = "Es obligatorio establecer una contraseña para tu cuenta. Esto te permitirá iniciar sesión también con tu correo y contraseña.";
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm, string? fromGoogle = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FromGoogle = !string.IsNullOrWhiteSpace(fromGoogle);
                return View(vm);
            }

            var usuarios = HttpContext.RequestServices.GetService<UsuariosData>();
            if (usuarios == null)
            {
                ModelState.AddModelError("", "Servicio de usuarios no disponible.");
                ViewBag.FromGoogle = !string.IsNullOrWhiteSpace(fromGoogle);
                return View(vm);
            }

            // Obtener el correo ANTES de consumir el token (porque después el token ya no es válido)
            var correoUsuario = await GetEmailFromResetTokenAsync(vm.Token, usuarios);
            
            // Hashear la contraseña antes de guardarla
            var passwordHash = ProyectoAeroline.Seguridad.PasswordHasher.Hash(vm.NuevaPassword);
            
            var (ok, code) = await usuarios.ConsumirTokenResetAsync(vm.Token, passwordHash);
            if (!ok)
            {
                ModelState.AddModelError("", code == "TOKEN_INVALIDO" ? "Token inválido o expirado." : "No fue posible restablecer la contraseña.");
                ViewBag.FromGoogle = !string.IsNullOrWhiteSpace(fromGoogle);
                return View(vm);
            }

            // Si viene de Google, hacer login automático después de cambiar contraseña
            if (!string.IsNullOrWhiteSpace(fromGoogle) && !string.IsNullOrWhiteSpace(correoUsuario))
            {
                var loginData = HttpContext.RequestServices.GetService<LoginData>();
                if (loginData != null)
                {
                    // Intentar login con el correo y la nueva contraseña para obtener los datos completos
                    var loginResult = await loginData.ValidarUsuarioAsync(correoUsuario, vm.NuevaPassword);
                    if (loginResult != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, loginResult.IdUsuario.ToString()),
                            new Claim(ClaimTypes.Name, loginResult.Nombre),
                            new Claim("Nombre", loginResult.Nombre),
                            new Claim(ClaimTypes.Email, loginResult.Correo),
                            new Claim(ClaimTypes.Role, loginResult.NombreRol)
                        };

                        var identity = new ClaimsIdentity(claims, AuthScheme);
                        await HttpContext.SignInAsync(
                            AuthScheme,
                            new ClaimsPrincipal(identity),
                            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) }
                        );

                        HttpContext.Session.SetInt32("IdUsuario", loginResult.IdUsuario);
                        HttpContext.Session.SetString("Nombre", loginResult.Nombre);
                        HttpContext.Session.SetString("Correo", loginResult.Correo);
                        HttpContext.Session.SetString("Rol", loginResult.NombreRol);

                        TempData["Info"] = $"¡Bienvenido {loginResult.Nombre}! Tu cuenta ha sido creada y tu contraseña ha sido establecida.";
                        return Redirect("/Index?from=google");
                    }
                }
            }

            TempData["Info"] = "Contraseña actualizada. Inicie sesión con su nueva contraseña.";
            return RedirectToAction("Login");
        }

        // Método auxiliar para crear token de reset directamente usando IdUsuario (para usuarios de Google)
        private async Task<string?> CrearTokenResetParaUsuarioAsync(int idUsuario, string correo, string? ip, string? userAgent)
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new SqlConnection(conn.GetConnectionString());
                await conexion.OpenAsync();

                // Crear token directamente usando IdUsuario
                var token = Guid.NewGuid().ToString();
                var fechaExpiracion = DateTime.UtcNow.AddHours(24); // 24 horas para usuarios de Google

                // Primero intentar con INSERT directo (más simple y confiable)
                using var cmdInsert = new SqlCommand(@"
                    -- Eliminar tokens previos no usados para este usuario
                    DELETE FROM [dbo].[PasswordResetTokens] 
                    WHERE [IdUsuario] = @IdUsuario AND [Usado] = 0;
                    
                    -- Insertar nuevo token
                    INSERT INTO [dbo].[PasswordResetTokens] 
                        ([IdUsuario], [Token], [FechaCreacion], [FechaExpiracion], [Usado], [IpSolicitud], [UserAgent])
                    VALUES 
                        (@IdUsuario, @Token, GETUTCDATE(), @FechaExpiracion, 0, @IpSolicitud, @UserAgent);
                    
                    SELECT @Token AS Token;
                ", conexion);
                
                cmdInsert.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmdInsert.Parameters.AddWithValue("@Token", token);
                cmdInsert.Parameters.AddWithValue("@FechaExpiracion", fechaExpiracion);
                cmdInsert.Parameters.AddWithValue("@IpSolicitud", (object?)ip ?? DBNull.Value);
                cmdInsert.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

                try
                {
                    var result = await cmdInsert.ExecuteScalarAsync();
                    var tokenResult = result?.ToString();
                    if (!string.IsNullOrWhiteSpace(tokenResult))
                    {
                        System.Diagnostics.Debug.WriteLine($"Token creado exitosamente usando IdUsuario: {idUsuario}");
                        return tokenResult;
                    }
                }
                catch (SqlException sqlEx) when (sqlEx.Number == 208 || sqlEx.Number == 2812) // Tabla no existe
                {
                    System.Diagnostics.Debug.WriteLine($"Tabla PasswordResetTokens no existe. Error: {sqlEx.Message}");
                    // Si la tabla no existe, retornar null para usar el método alternativo
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al insertar token directo: {ex.Message}");
                    // Continuar con el intento alternativo
                }

                // Si el INSERT directo falla, intentar usando el stored procedure con el correo
                // (aunque puede que no encuentre al usuario recién creado)
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearTokenResetParaUsuarioAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        // Método auxiliar para obtener el correo desde el token de reset (sin consumirlo)
        private async Task<string?> GetEmailFromResetTokenAsync(string token, UsuariosData usuarios)
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new SqlConnection(conn.GetConnectionString());
                await conexion.OpenAsync();

                using var cmd = new SqlCommand(@"
                    SELECT TOP 1 U.[Correo]
                    FROM [dbo].[PasswordResetTokens] PRT
                    INNER JOIN [dbo].[Usuarios] U ON PRT.[IdUsuario] = U.[IdUsuario]
                    WHERE PRT.[Token] = @Token 
                      AND PRT.[Usado] = 0
                      AND PRT.[FechaExpiracion] > GETUTCDATE()
                ", conexion);
                cmd.Parameters.AddWithValue("@Token", token);

                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString();
            }
            catch
            {
                return null;
            }
        }

        // ===================================================
        // ================== CONFIRMAR EMAIL =================
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

        // ===================================================
        // ============ CONFIRMAR LOGIN CON GOOGLE ===========
        // ===================================================

        [HttpGet("ConfirmGoogleLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmGoogleLogin(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    TempData["Error"] = "Token de confirmación no proporcionado.";
                    return RedirectToAction("Login");
                }

                // Verificar que el token sea válido antes de consumirlo
                var tokenValido = await _usuariosData.VerificarTokenConfirmacionGoogleAsync(token);
                if (!tokenValido)
                {
                    TempData["Error"] = "El enlace de confirmación ha expirado o no es válido. Por favor, intenta iniciar sesión con Google nuevamente.";
                    return RedirectToAction("Login");
                }

                // Confirmar y crear el usuario
                var usuario = await _usuariosData.ConfirmarGoogleLoginAsync(token);
                
                if (usuario == null)
                {
                    TempData["Error"] = "No se pudo crear tu cuenta. El enlace puede haber expirado o ya fue usado.";
                    return RedirectToAction("Login");
                }

                // Usuario creado exitosamente - NO hacer login automático
                // En su lugar, crear token de reset de contraseña y redirigir DIRECTAMENTE a cambiar contraseña
                // Esto es OBLIGATORIO para usuarios de Google
                var correoFinal = usuario.Correo ?? "";
                
                if (string.IsNullOrWhiteSpace(correoFinal))
                {
                    TempData["Error"] = "No se pudo obtener el correo del usuario. Por favor, intenta iniciar sesión con Google nuevamente.";
                    return RedirectToAction("Login");
                }

                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers.UserAgent.ToString();

                // Crear token de reset de contraseña OBLIGATORIO para usuarios de Google
                // Primero intentar con el método estándar (busca por correo)
                var (resetToken, _) = await _usuariosData.CrearTokenResetAsync(correoFinal, ip, userAgent);
                
                // Si falla, crear directamente usando IdUsuario
                if (string.IsNullOrWhiteSpace(resetToken))
                {
                    resetToken = await CrearTokenResetParaUsuarioAsync(usuario.IdUsuario, correoFinal, ip, userAgent);
                }
                
                if (string.IsNullOrWhiteSpace(resetToken))
                {
                    // Si todo falla, intentar una última vez con un pequeño delay para asegurar que el usuario esté en la BD
                    await Task.Delay(500); // Esperar medio segundo
                    var (retryToken, _) = await _usuariosData.CrearTokenResetAsync(correoFinal, ip, userAgent);
                    
                    if (string.IsNullOrWhiteSpace(retryToken))
                    {
                        System.Diagnostics.Debug.WriteLine($"No se pudo crear token de reset. IdUsuario: {usuario.IdUsuario}, Correo: {correoFinal}");
                        // Aunque falló, redirigir al Login con mensaje informativo
                        // El usuario puede usar "Olvidé mi contraseña" después
                        TempData["Info"] = $"Tu cuenta ha sido creada. Para establecer tu contraseña, usa 'Olvidé mi contraseña' desde el Login con tu correo: {correoFinal}";
                        return RedirectToAction("Login");
                    }
                    resetToken = retryToken;
                }

                // Redirigir DIRECTAMENTE a la página de cambio de contraseña con el token y un parámetro especial para indicar que viene de Google
                // NO usar View(), siempre usar Redirect
                return RedirectToAction("ResetPassword", "Account", new { token = resetToken, fromGoogle = "true" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ConfirmGoogleLogin: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Ocurrió un error al procesar la confirmación. Por favor, intenta iniciar sesión con Google nuevamente.";
                return RedirectToAction("Login");
            }
        }

        // Método auxiliar para obtener el IdRol de Usuario dinámicamente
        private async Task<int> ObtenerIdRolUsuarioAsync()
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new Microsoft.Data.SqlClient.SqlConnection(conn.GetConnectionString());
                await conexion.OpenAsync();
                
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT TOP 1 [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Usuario' AND [FechaEliminacion] IS NULL ORDER BY [IdRol]", 
                    conexion);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                // Si falla, retornar 5 como fallback
            }
            return 5; // Fallback si no se encuentra
        }
    }
}
