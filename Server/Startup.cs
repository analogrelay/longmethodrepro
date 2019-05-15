using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                string GetSharedFxVersion(Type type)
                {
                    var asmPath = type.Assembly.Location;
                    var versionFile = Path.Combine(Path.GetDirectoryName(asmPath), ".version");

                    var simpleVersion = File.Exists(versionFile) ?
                        File.ReadAllLines(versionFile).Last() :
                        "<unknown>";

                    var infoVersion = type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "<unknown>";

                    return $"{simpleVersion} ({infoVersion})";
                }

                if (context.Request.Path.StartsWithSegments("/.runtime-info"))
                {
                    context.Response.ContentType = "text/plain";
                    var aspnetCoreVersion = GetSharedFxVersion(typeof(IApplicationBuilder));
                    var netCoreVersion = GetSharedFxVersion(typeof(string));
                    await context.Response.WriteAsync($"ASP.NET Core Runtime version: {aspnetCoreVersion}{Environment.NewLine}");
                    await context.Response.WriteAsync($".NET Core Runtime version: {netCoreVersion}{Environment.NewLine}");
                }
                else
                {
                    await next();
                }
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Broadcaster>("/broadcaster");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
