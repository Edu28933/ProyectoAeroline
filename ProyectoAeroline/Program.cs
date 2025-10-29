using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// MVC (Views/*) y Razor Pages (Pages/*)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// (Opcional, pero recomendado si luego har�s login real con cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        opt.AccessDeniedPath = "/Account/Login";
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: autenticaci�n antes de autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); // Habilita públicamente

// Redirige "/" al Login MVC
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

// Rutas MVC por defecto (Account/Login)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

// Habilita Razor Pages (por si usas /Pages/Index.cshtml, etc.)
app.MapRazorPages();

app.Run();


