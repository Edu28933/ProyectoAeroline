// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost] // ESTA acción evita el 405
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        // TODO: valida contra tu base de datos/Identity
        var ok = model.Email == "demo@demo.com" && model.Password == "123456";

        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(model);
        }

        // Login con cookies (ejemplo mínimo)
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, model.Email),
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, "Usuario")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}

public class LoginViewModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}
