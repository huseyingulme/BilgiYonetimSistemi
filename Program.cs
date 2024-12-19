using BilgiYonetimSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Veritabaný baðlantýsýný ayarlama
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // HTTP istemcisi fabrikasýný ekleme
        builder.Services.AddHttpClient(); // IHttpClientFactory servisini ekledik

        builder.Services.AddControllersWithViews();

        // Loglama servislerini ekleme
        builder.Services.AddLogging(options =>
        {
            options.AddConsole();
            options.AddDebug();
            options.AddEventSourceLogger();
        });

        // Session ve Cache yapýlandýrmasý
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.IsEssential = true;
        });

        // Swagger ayarlarý
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BilgiYonetimSistemi", Version = "v1" });
        });

        var app = builder.Build();


        // Development ortamý için Swagger ve hata sayfasý
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

        // Route ayarlarý
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}");

        app.Run();
    }
}