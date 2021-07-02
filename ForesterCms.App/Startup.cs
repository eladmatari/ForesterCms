using Common.Utils;
using Common.Utils.Logging;
using Common.Utils.Standard;
using ForesterCmsServices.Objects;
using ForesterCmsServices.UI.General;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            if (Config.Environment == EnvironmentType.Local)
                services.AddControllersWithViews().AddRazorRuntimeCompilation().AddNewtonsoftJson();
            else
                services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Router>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "ForesterCms";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "ForesterCmsSession";
                //options.Cookie.MaxAge = TimeSpan.FromDays(7);
            });
            services.AddDataProtection()
                .SetDefaultKeyLifetime(DateTime.Now.AddDays(8).Date.AddHours(3).Subtract(DateTime.Now))
                .AddKeyManagementOptions(options => options.XmlRepository = new SqlXmlRepository())
                .SetApplicationName("ForesterCms");

            if (Config.Environment == EnvironmentType.Local)
            {
                services.AddLogging((configure) =>
                {
                    configure.SetMinimumLevel(LogLevel.Trace);
                    configure.AddProvider(new AppLoggerProvider());
                });
            }
            else
            {
                services.AddResponseCompression();
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                if (CmsConfig.IsSite)
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                }

                if (CmsConfig.IsCms)
                {
                    endpoints.MapAreaControllerRoute("ForesterCms", "ForesterCms", "ForesterCms/{controller=Home}/{action=Index}/{id?}");
                }
                //endpoints.MapDynamicControllerRoute<Router>("{*url}");
            });
        }
    }
}
