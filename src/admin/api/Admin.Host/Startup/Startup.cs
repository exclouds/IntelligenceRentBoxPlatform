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
            //��ӡ��Ҫ������Ϣ
            _logger.LogInformation($"Environment:{env.EnvironmentName}{Environment.NewLine}" +
                                   $"ConnectionString:{_appConfiguration["ConnectionStrings:Default"]}{Environment.NewLine}" +
                                   $"RedisCache:IsEnabled:{_appConfiguration["Abp:RedisCache:IsEnabled"]}  ConnectionString:{_appConfiguration["Abp:RedisCache:ConnectionString"]}{Environment.NewLine}" +
                                   $"SignalRRedisCache:{_appConfiguration["Abp:SignalRRedisCache:ConnectionString"]}{Environment.NewLine}" +
                                   $"HTTPS:HttpsRedirection:{_appConfiguration["App:HttpsRedirection"]}  UseHsts:{_appConfiguration["App:UseHsts"]}{Environment.NewLine}" +
                                   $"CorsOrigins:{_appConfiguration["App:CorsOrigins"]}{Environment.NewLine}" +
                                   $"RabbitMQ:{_appConfiguration["RabbitMQ:Default"]}{Environment.NewLine}");

        }

        /// <summary>
        /// �����Զ������
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
            //	//�����ʹ�õ� EF �������ݲ���������Ҫ����������ã�
            //	x.UseEntityFramework<AdminDbContext>();  //��ѡ��㲻��Ҫ�ٴ����� x.UseSqlServer ��

            //	//�����ʹ�õ�ADO.NET���������ݿ�ѡ��������ã�
            //	x.UseSqlServer(_appConfiguration["ConnectionStrings:Default"]);
            //	//x.UseMySql("���ݿ������ַ���");
            //	//x.UsePostgreSql("���ݿ������ַ���");

            //	//�����ʹ�õ� MongoDB�����������������ã�
            //	//x.UseMongoDB("ConnectionStrings");  //ע�⣬��֧��MongoDB 4.0+��Ⱥ

            //	//CAP֧�� RabbitMQ��Kafka��AzureServiceBus ����ΪMQ������ʹ��ѡ�����ã�
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
            //��������ȫ�����󣬲�������
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()//��������վ���������net core2.2�汾�󽫲����ã�
                        .AllowAnyHeader()// ������������ͷ
                        .AllowAnyMethod()// �����������󷽷�
                        .AllowCredentials();// ����Cookie��Ϣ
                });
            });
            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                //���鿪���������������ʾ��ȫͼ��
                //����https�ض���˿�
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            //�Ƿ�����HTTP�ϸ��䰲ȫЭ��(HSTS)
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
                _logger.LogError("ִ��ConfigureCustomServices���ִ���", ex);
            }

            try
            {
                _logger.LogWarning("abp  Begin...");
                //����ABP�Լ����ģ������
                return services.AddAbp<AdminWebHostModule>(options =>
                {

                    options.IocManager.Register<IAppConfigurationAccessor, AppConfigurationAccessor>(DependencyLifeStyle
                        .Singleton);

                    //������־
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

                    //Ĭ�ϲ��������Ŀ¼�����Ƽ����ģʽ��
                    //options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError("����Abp���ִ���", ex);
                return null;
            }

        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //ʹ��TimedJob
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
            //    c.SwaggerEndpoint("DBService/swagger.json", "���ݷ���Pro");
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
                ////ʹ�ó���ѯ
                //routes.MapHub<AbpCommonHub>("/signalr", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
                //routes.MapHub<ChatHub>("/signalr-chat", otp => otp.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling);
            });

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                _logger.LogWarning("׼������HTTS��ת...");
                //���鿪���������������ʾ��ȫͼ��
                app.UseHttpsRedirection();
            }

            //�Ƿ�����HTTP�ϸ��䰲ȫЭ��(HSTS)�����������رա�
            if (!env.IsDevelopment() && bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                _logger.LogWarning("׼������HSTS...");
                try
                {
                    app.UseHsts();
                    _logger.LogWarning("�ɹ�����HSTS...");
                }
                catch (Exception ex)
                {
                    _logger.LogError("����HSTS���ִ���", ex);
                }
            }

            try
            {
                _logger.LogWarning("Ӧ���Զ�������...");
                CustomConfigure(app, env, loggerFactory);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ӧ���Զ������ó��ִ���", ex);
            }
            if (Convert.ToBoolean(_appConfiguration["Consul:IsEnabled"]))
            {
                //����ע��
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
