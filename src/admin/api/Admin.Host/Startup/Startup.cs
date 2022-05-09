using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Castle.Logging.NLog;
using Abp.Dependency;
using Abp.Extensions;
using Castle.Facilities.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Identity;
using Magicodes.Admin.Web.Chat.SignalR;
using Magicodes.Admin.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Abp.Json;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Pomelo.AspNetCore.TimedJob;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Web.Extensions;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Magicodes.Admin.Web.Startup
{
    public partial class Startup
    {
        private readonly ILogger _logger;
        private const string DefaultCorsPolicyName = "http://192.168.5.199";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
            _logger = logger;
            //打印主要配置信息
            _logger.LogInformation($"Environment:{env.EnvironmentName}{Environment.NewLine}" +
                                   $"ConnectionString:{_appConfiguration["ConnectionStrings:Default"]}{Environment.NewLine}" +
                                   $"RedisCache:IsEnabled:{_appConfiguration["Abp:RedisCache:IsEnabled"]}  ConnectionString:{_appConfiguration["Abp:RedisCache:ConnectionString"]}{Environment.NewLine}" +
                                   $"SignalRRedisCache:{_appConfiguration["Abp:SignalRRedisCache:ConnectionString"]}{Environment.NewLine}" +
                                   $"HTTPS:HttpsRedirection:{_appConfiguration["App:HttpsRedirection"]}  UseHsts:{_appConfiguration["App:UseHsts"]}{Environment.NewLine}" +
                                   $"CorsOrigins:{_appConfiguration["App:CorsOrigins"]}{Environment.NewLine}" +
                                   $"RabbitMQ:{_appConfiguration["RabbitMQ:Default"]}{Environment.NewLine}");

        }

        /// <summary>
        /// 配置自定义服务
        /// </summary>
        /// <param name="services"></param>
        partial void ConfigureCustomServices(IServiceCollection services);

        partial void CustomConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory);

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTimedJob();
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #region Cap 
            services.AddDbContext<AdminDbContext>();
            

            //services.AddCap(x =>
            //{
            //	//如果你使用的 EF 进行数据操作，你需要添加如下配置：
            //	x.UseEntityFramework<AdminDbContext>();  //可选项，你不需要再次配置 x.UseSqlServer 了

            //	//如果你使用的ADO.NET，根据数据库选择进行配置：
            //	x.UseSqlServer(_appConfiguration["ConnectionStrings:Default"]);
            //	//x.UseMySql("数据库连接字符串");
            //	//x.UsePostgreSql("数据库连接字符串");

            //	//如果你使用的 MongoDB，你可以添加如下配置：
            //	//x.UseMongoDB("ConnectionStrings");  //注意，仅支持MongoDB 4.0+集群

            //	//CAP支持 RabbitMQ、Kafka、AzureServiceBus 等作为MQ，根据使用选择配置：
            //	x.UseRabbitMQ(_appConfiguration["RabbitMQ:Default"]);
            //	x.UseDashboard();
            //	//x.UseKafka("ConnectionStrings");
            //	//x.UseAzureServiceBus("ConnectionStrings");
            //});
            #endregion
            services.AddHealthChecks();

            var sbuilder = services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            if (!_appConfiguration["Abp:SignalRRedisCache:ConnectionString"].IsNullOrWhiteSpace())
            {
                _logger.LogWarning("Abp:SignalRRedisCache:ConnectionString:" + _appConfiguration["Abp:SignalRRedisCache:ConnectionString"]);
                sbuilder.AddRedis(_appConfiguration["Abp:SignalRRedisCache:ConnectionString"]);
            }
            //跨域，允许全部请求，不加限制
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()//允许所有站点跨域请求（net core2.2版本后将不适用）
                        .AllowAnyHeader()// 允许所有请求头
                        .AllowAnyMethod()// 允许所有请求方法
                        .AllowCredentials();// 允许Cookie信息
                });
            });
            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                //建议开启，以在浏览器显示安全图标
                //设置https重定向端口
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            //是否启用HTTP严格传输安全协议(HSTS)
            if (bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                //services.AddHsts(options =>
                //{
                //    options.Preload = true;
                //    options.IncludeSubDomains = true;
                //    options.MaxAge = TimeSpan.FromDays(60);
                //    options.ExcludedHosts.Add("example.com");
                //});
            }

            try
            {
                _logger.LogWarning("ConfigureCustomServices  Begin...");
                ConfigureCustomServices(services);
                _logger.LogWarning("ConfigureCustomServices  End...");
            }
            catch (Exception ex)
            {
                _logger.LogError("执行ConfigureCustomServices出现错误", ex);
            }

            try
            {
                _logger.LogWarning("abp  Begin...");
                //配置ABP以及相关模块依赖
                return services.AddAbp<AdminWebHostModule>(options =>
                {

                    options.IocManager.Register<IAppConfigurationAccessor, AppConfigurationAccessor>(DependencyLifeStyle
                        .Singleton);

                    //配置日志
                    options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                        f =>
                        {
                            var logType = _appConfiguration["Abp:LogType"];
                            _logger.LogInformation($"LogType:{logType}");
                            if (logType != null && logType == "NLog")
                            {
                                f.UseAbpNLog().WithConfig("nlog.config");
                            }
                            else
                            {
                                f.UseAbpLog4Net().WithConfig("log4net.config");
                            }
                        });

                    //默认不启动插件目录（不推荐插件模式）
                    //options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError("配置Abp出现错误", ex);
                return null;
            }

        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //使用TimedJob
            //app.UseTimedJob();

            app.UseHealthChecks("/health");

            //Initializes ABP framework.
            try
            {
                app.UseAbp(options =>
                {
                    options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
                });
            }
            catch (Exception ex)
            {

                throw;
            }


            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
            }
            app.UseSwagger();

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("DBService/swagger.json", "数据服务Pro");
            //});


            app.UseStaticFiles();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    app.UseAbpRequestLocalization();
                }
            }

            //app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
                routes.MapHub<ChatHub>("/signalr-chat");
                ////使用长轮询
                //routes.MapHub<AbpCommonHub>("/signalr", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
                //routes.MapHub<ChatHub>("/signalr-chat", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
            });

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                _logger.LogWarning("准备启用HTTS跳转...");
                //建议开启，以在浏览器显示安全图标
                app.UseHttpsRedirection();
            }

            //是否启用HTTP严格传输安全协议(HSTS)【开发环境关闭】
            if (!env.IsDevelopment() && bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                _logger.LogWarning("准备启用HSTS...");
                try
                {
                    app.UseHsts();
                    _logger.LogWarning("成功启用HSTS...");
                }
                catch (Exception ex)
                {
                    _logger.LogError("启用HSTS出现错误", ex);
                }
            }

            try
            {
                _logger.LogWarning("应用自定义配置...");
                CustomConfigure(app, env, loggerFactory);
            }
            catch (Exception ex)
            {
                _logger.LogError("应用自定义配置出现错误", ex);
            }
            if (Convert.ToBoolean(_appConfiguration["Consul:IsEnabled"]))
            {
                //服务注册
                var consulExtensionsModel = new ConsulExtensionsModel()
                {
                    IP = _appConfiguration["Consul:IP"],
                    Port = Convert.ToInt32(_appConfiguration["Consul:Port"]),
                    Protocol = _appConfiguration["Consul:Protocol"]
                };
                var serviceNode = new ServiceNodeExtensionsModel()
                {
                    IP = _appConfiguration["Consul:CurrentServiceNode:IP"],
                    Port = Convert.ToInt32(_appConfiguration["Consul:CurrentServiceNode:Port"]),
                    Name = _appConfiguration["Consul:CurrentServiceNode:Name"],
                    Protocol = _appConfiguration["Consul:CurrentServiceNode:Protocol"],
                    IsEnabledHealthCheck = Convert.ToBoolean(_appConfiguration["Consul:CurrentServiceNode:IsEnabledHealthCheck"])
                };
                app.RegisterConsul(lifetime, serviceNode, consulExtensionsModel);
            }
        }
    }
}
