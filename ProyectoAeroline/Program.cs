var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();            // para Pages/Index.cshtml
builder.Services.AddControllersWithViews();  // para Views/Empleados/Listar.cshtml

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseStaticFiles(); // Habilita públicamente

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // habilita /Empleados/Listar

app.MapRazorPages(); // habilita /Index

app.Run();


