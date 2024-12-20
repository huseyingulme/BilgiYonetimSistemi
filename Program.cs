using BilgiYonetimSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddHttpClient();
        builder.Services.AddControllersWithViews();
        builder.Services.AddLogging(options =>
        {
            options.AddConsole();
            options.AddDebug();
            options.AddEventSourceLogger();
        });
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BilgiYonetimSistemi", Version = "v1" });
        });

        var app = builder.Build();

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
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}");
        app.Run();
    }
}
