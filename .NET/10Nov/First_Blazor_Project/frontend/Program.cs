using frontend.Components;
using Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;


namespace frontend

{

    public class Program

    {

        public static void Main(string[] args)

        {

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddQuickGridEntityFrameworkAdapter();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Add services to the container.

            builder.Services.AddRazorComponents()

                .AddInteractiveServerComponents();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7223/api/") });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())

            {

                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHsts();

                app.UseMigrationsEndPoint();

            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAntiforgery();

            app.MapRazorComponents<App>()

                .AddInteractiveServerRenderMode();

            app.Run();

        }

    }

}