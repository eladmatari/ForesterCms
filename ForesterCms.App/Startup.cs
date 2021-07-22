using Common.Utils;
using Common.Utils.Logging;
using Common.Utils.Standard;
using ForesterCmsServices.Objects;
using ForesterCmsServices.UI.Base;
using ForesterCmsServices.UI.General;
using ForesterCmsServices.UI.ImageMagick;
using ForesterCmsServices.UI.Resources;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, IServiceProvider svp)
        {
            if (Config.Environment == EnvironmentType.Local)
            {
                app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions()
                {
                    SourceCodeLineCount = 10
                });

                app.UseBrowserLink();
            }
            else
            {
                app.UseResponseCompression();
                app.UseHsts();
            }

            DiHelper.Configure(svp);
            HttpContextHelper.Configure(svp.GetRequiredService<IHttpContextAccessor>());

            app.UseSession();
            app.UseImageMagick((options) =>
            {
                AddMagickProcesses(options);
            });

            BaseMiddleware.OnError = Application_Error;
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseRewriter(new RewriteOptions().AddStaticRewrite());
            app.UseMiddleware<BaseMiddleware>();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".scss", "text/x-sass");

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Config.GetAppSettings("ForesterCms.UploadedFilesDirectory"), "$Images")),
                RequestPath = "/uploadedimages"
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Config.GetAppSettings("ForesterCms.UploadedFilesDirectory")),
                RequestPath = "/uploadedfiles"
            });

            app.UseCookiePolicy();
            app.UseRouting();

            // For most apps, calls to UseAuthentication, UseAuthorization, and UseCors must appear between the calls to UseRouting and UseEndpoints to be effective.

            app.UseEndpoints(endpoints =>
            {
                if (CmsConfig.IsCms)
                {
                    endpoints.MapAreaControllerRoute("ForesterCms", "ForesterCms", "ForesterCms/{controller=Home}/{action=Index}/{id?}");
                }

                if (CmsConfig.IsSite)
                {
                    endpoints.MapDynamicControllerRoute<Router>("{*url}");
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapFallbackToController("Error404", "General");
                }
            });

            Application_Start();
            applicationLifetime.ApplicationStopping.Register(Application_End);
        }

        private void Application_Start()
        {
            Logger.Info("!-----------------------------------------------------");

            ResourceGroupHelper.Scripts.StartWatch();
            ResourceGroupHelper.Css.StartWatch();
            DbLogger.Instance.Start();
            //CacheRefresherProcess.Instance.Start();
            //SearchLogsProcess.Instance.Start();
            //LogsCacheRefresherProcess.Instance.Start();
            //eGenServices.Logic.Extensions.OnLinkProcess = OnLinkProcess;

            SetVirtualRoutes();
        }

        private void SetVirtualRoutes()
        {
            Router.AddRoute(new VirtualRoute($"product",
                new VirtualRouteParam("Name")
            ));

            Router.AddRouteTranslate(new VirtualRouteTranslate($"other/product", $"product"));
        }

        private void Application_End()
        {
            ResourceGroupHelper.Scripts.StopWatch();
            ResourceGroupHelper.Css.StopWatch();
            DbLogger.Instance.Stop();
            //CacheRefresherProcess.Instance.Stop();
            //SearchLogsProcess.Instance.Stop();
            //LogsCacheRefresherProcess.Instance.Stop();

            Logger.Info("-----------------------------------------------------!");
        }

        private void AddMagickProcesses(ImageMagickMiddlewareOptions options)
        {
            string uploadedImagesRequestPath = "/uploadedimages";
            var uploadedImagesFilesProvider = new PhysicalFileProvider(Config.GetAppSettings("ForesterCms.UploadedFilesDirectory"));

            options.AddProcess(new ImageMagickProcess()
            {
                Key = "type1",
                Width = 253,
                Height = 155,
                Mode = ImageMagickMode.ResizeMax,
                FileProvider = uploadedImagesFilesProvider,
                RequestPath = uploadedImagesRequestPath
            });
        }

        private void Application_Error(Exception ex)
        {
            Logger.Error(ex, "application");
            DbLogger.AddLog(ex, "application", null, null, null, null, null);
        }
    }
}
