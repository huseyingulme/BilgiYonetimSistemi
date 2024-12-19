using BilgiYonetimSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Veritaban� ba�lant�s�n� ayarlama
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // HTTP istemcisi fabrikas�n� ekleme
        builder.Services.AddHttpClient(); // IHttpClientFactory servisini ekledik

        builder.Services.AddControllersWithViews();

        // Loglama servislerini ekleme
        builder.Services.AddLogging(options =>
        {
            options.AddConsole();
            options.AddDebug();
            options.AddEventSourceLogger();
        });

        // Session ve Cache yap�land�rmas�
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.IsEssential = true;
        });

        // Swagger ayarlar�
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BilgiYonetimSistemi", Version = "v1" });
        });

        var app = builder.Build();


        // Development ortam� i�in Swagger ve hata sayfas�
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BilgiYonetimSistemi v1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();

        // Route ayarlar�
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}");

        app.Run();
    }
}