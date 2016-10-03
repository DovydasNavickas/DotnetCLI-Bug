using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLib;

namespace ZipTest
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new HttpClient());
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment,
            ILoggerFactory factory, ILogger<Startup> logger, HttpClient httpClient)
        {
            factory.AddConsole(LogLevel.Debug);
            factory.AddDebug();

            app.UseDeveloperExceptionPage();

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/zip"))
                {
                    await next();
                    return;
                }

                using (var zip = new MemoryStream())
                {
                    const string photoUri = "https://c.s-microsoft.com/lt-lt/CMSImages/O16_PlanHero_E5_1920x660.jpg?version=7091bbcb-6f35-bbf3-70ff-2ade19f0aabe";
                    ZippingHelper.CreateZipWithItem(zip, await httpClient.GetStreamAsync(new Uri(photoUri)));
                    zip.Seek(0, SeekOrigin.Begin);
                    var zipName = "test";
                    context.Response.Headers.Add("Content-Disposition", $"attachment; filename={zipName}.zip");
                    await zip.CopyToAsync(context.Response.Body);
                }
            });

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/ziplocal"))
                {
                    await next();
                    return;
                }

                using (var zip = new MemoryStream())
                {
                    const string photoUri = "https://c.s-microsoft.com/lt-lt/CMSImages/O16_PlanHero_E5_1920x660.jpg?version=7091bbcb-6f35-bbf3-70ff-2ade19f0aabe";
                    ZipHelperInTheSameProject.CreateZipWithItem(zip, await httpClient.GetStreamAsync(new Uri(photoUri)));
                    zip.Seek(0, SeekOrigin.Begin);
                    var zipName = "test";
                    context.Response.Headers.Add("Content-Disposition", $"attachment; filename={zipName}.zip");
                    await zip.CopyToAsync(context.Response.Body);
                }
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello world of pain localizing this issue.");
            });
        }
    }
}