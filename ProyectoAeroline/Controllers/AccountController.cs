using ProyectoAeroline;                  // LoginViewModel
using ProyectoAeroline.Data;             // LoginData
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
        public AccountController(LoginData loginData) => _loginData = loginData;

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



        // ========== LOGIN (POST) ==========
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
                new Claim(ClaimTypes.Name, nombre),             // <- usado por User.Identity.Name
                new Claim("Nombre", nombre),                    // <- opcional, por si lo lees como "Nombre"
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

        // ===== LOGOUT (GET) =====
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

        // ===== LOGOUT (POST) =====
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            foreach (var c in Request.Cookies.Keys) Response.Cookies.Delete(c);

            return RedirectToAction("Login");
        }
    }
}
