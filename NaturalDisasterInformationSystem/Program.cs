using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using NaturalDisasterInformationSystem.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set license context

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DO_ANContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));
builder.Services.AddHttpClient();

// C?u hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian timeout
    options.Cookie.HttpOnly = true; // Cookie ch? ???c truy c?p t? HTTP
    options.Cookie.IsEssential = true; // Cookie c?n thi?t ?? ho?t ??ng
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddScoped<SMSService>(provider =>
{
    var context = provider.GetRequiredService<DO_ANContext>();
    var apiKey = "2F54C9036C33CD8A92D750EBD3E954";
    var secretKey = "714AD4B9DC4A88AE6CD97D772F975E";

    return new SMSService(apiKey, secretKey, context);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSession(); // Kích ho?t Session

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/user/home");
        return; 
    }
    await next(); 
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
    );
});

app.MapRazorPages();

app.Run();
