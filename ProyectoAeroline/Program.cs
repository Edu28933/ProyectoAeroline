using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;               // <- necesario para CookieSecurePolicy / SameSiteMode
using ProyectoAeroline.Data;

var builder = WebApplication.CreateBuilder(args);



// ------- Session -------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
    opt.Cookie.Name = ".ProyectoAeroline.Session";
});


// ------- MVC + filtro global [Authorize] -------
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));   // <- fuerza auth en TODAS las acciones MVC
});

// ------- Razor Pages -------
builder.Services.AddRazorPages(options =>
{
    // Autoriza todo el árbol de Pages
    options.Conventions.AuthorizeFolder("/");

    // Si alguna Page debe ser pública, exímela aquí.
    // options.Conventions.AllowAnonymousToPage("/Account/Login");
});

// DI para tu clase de acceso al SP
builder.Services.AddScoped<LoginData>();

// Cookies de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        opt.AccessDeniedPath = "/Account/Login";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// ------- Política GLOBAL por si algo se escapa -------
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Antiforgery compatible con HTTP local (dev)
builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "Aero.AntiForgery";                    // sin __Host- en dev
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // acepta HTTP en localhost
    o.Cookie.SameSite = SameSiteMode.Lax;                  // Lax para formularios
    o.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

// ------- CookiePolicy (antes de Routing/Auth) -------
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseRouting();

app.UseAuthentication();   // Autenticación primero
app.UseAuthorization();    // Luego autorización
app.UseSession();          // Sesión después de Auth

// Evita cachear contenido autenticado (Back/Forward cache)
// IMPORTANTE: Establecer headers ANTES de await next() para evitar el error "Headers are read-only"
app.Use(async (ctx, next) =>
{
    // Establecer headers ANTES de procesar la respuesta
    // Los headers deben establecerse antes de que cualquier middleware escriba en el response
    if (!ctx.Response.HasStarted && ctx.User?.Identity?.IsAuthenticated == true)
    {
        ctx.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        ctx.Response.Headers["Pragma"] = "no-cache";
        ctx.Response.Headers["Expires"] = "0";
    }
    
    await next();
});

// Redirige "/" al Login
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

// ------- ENDPOINTS protegidos por defecto -------

// Rutas MVC convencionales (Login se libera con [AllowAnonymous])
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
).RequireAuthorization();

// Razor Pages
app.MapRazorPages().RequireAuthorization();

app.Run();
