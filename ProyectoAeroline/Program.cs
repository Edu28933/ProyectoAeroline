using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;               // <- necesario para CookieSecurePolicy / SameSiteMode
using Microsoft.AspNetCore.Authentication;     // <-- NUEVO (para AddGoogle)
using ProyectoAeroline.Data;
using ProyectoAeroline.Services;               // <-- NUEVO (servicio de email)

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

// ==== NUEVO: servicios requeridos por las nuevas funciones ====
builder.Services.AddScoped<IEmailService, EmailService>(); // envío de correos (reset/verificación)
builder.Services.AddScoped<UsuariosData>(); // acceso a datos de usuarios y Google login
builder.Services.AddScoped<RolPantallaOpcionData>(); // acceso a permisos
builder.Services.AddScoped<ProyectoAeroline.Services.IPermisosService, ProyectoAeroline.Services.PermisosService>(); // servicio de permisos

// Cookies de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = ".Aeroline.Auth";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Lax;              // OK para login con redirección
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; // SIEMPRE https
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        opt.AccessDeniedPath = "/Account/Login";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    })
    .AddGoogle("Google", opt =>
    {
        opt.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        opt.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        opt.CallbackPath = "/signin-google";                 // igual que en Google Console
        opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.SaveTokens = true;
        opt.Scope.Add("profile");
        opt.Scope.Add("email");
        opt.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Account/Login?error=access_denied");
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                // Manejar error de correlación u otros errores de OAuth
                context.HandleResponse();
                var error = context.Failure?.Message ?? "Error desconocido";
                
                // Si es error de correlación, limpiar y redirigir
                if (error.Contains("Correlation", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("/Account/Login?error=correlation_failed&message=" + Uri.EscapeDataString("La sesión de autenticación expiró. Por favor, intenta iniciar sesión con Google nuevamente."));
                }
                else
                {
                    context.Response.Redirect("/Account/Login?error=oauth_failed&message=" + Uri.EscapeDataString(error));
                }
                
                return Task.CompletedTask;
            }
        };
    });

// (Opcional) No fuerces SameSite global.
// Si mantienes CookiePolicy, que NO baje a Lax cosas que el provider necesite en None.





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

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax // <- OK, pero no uses 'Strict'
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // Autenticación primero
app.UseAuthorization();    // Luego autorización
app.UseSession();          // Sesión después de Auth

// NOTA: No necesitamos middleware adicional para headers de cache
// Los controladores y páginas ya usan [ResponseCache(NoStore = true)] para evitar cache

// Redirige "/" al Login
app.MapGet("/", ctx =>
{
    if (ctx.User?.Identity?.IsAuthenticated == true)
        ctx.Response.Redirect("/Index");
    else
        ctx.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});
    


// ------- ENDPOINTS protegidos por defecto -------

// Rutas MVC convencionales (Login se libera con [AllowAnonymous])
// Rutas MVC convencionales
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
); // <- SIN RequireAuthorization()

// Razor Pages
app.MapRazorPages(); // <- SIN RequireAuthorization()


app.Run();
