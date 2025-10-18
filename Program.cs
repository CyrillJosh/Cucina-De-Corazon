using Cucina_De_Corazon.Context;
using Microsoft.EntityFrameworkCore;

namespace Cucina_De_Corazon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //Database
            builder.Services.AddDbContext<MyDBContext>(options =>
                options
                    .UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Default").Value,
                        sql => sql.EnableRetryOnFailure())
                    .EnableSensitiveDataLogging(),
                ServiceLifetime.Transient
            );
            //SMTP
            builder.Services.AddScoped<IEmailService, EmailService>();
            //Session service
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //Session
            app.UseSession();
            app.UseRouting();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
