using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Extensions
{
    /// <summary>
    /// 服务注册扩展
    /// </summary>
    public static class ConsulBuilderExtensions
    {
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <param name="serviceNodeExtensionsModel"></param>
        /// <param name="consulExtensionsModel"></param>
        /// <returns></returns>
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, ServiceNodeExtensionsModel serviceNodeExtensionsModel, ConsulExtensionsModel consulExtensionsModel)
        {
            var consulClient = new ConsulClient(x => x.Address = new Uri($"{consulExtensionsModel.Protocol}://{consulExtensionsModel.IP}:{consulExtensionsModel.Port}"));//请求注册的 Consul 地址


            
            // Consul注册服务
            var registration = new AgentServiceRegistration()
            {
                ID = serviceNodeExtensionsModel.Name + "_" + serviceNodeExtensionsModel.Port,
                Name = serviceNodeExtensionsModel.Name,
                Address = serviceNodeExtensionsModel.IP,
                Port = serviceNodeExtensionsModel.Port,
                Tags = new[] { $"urlprefix-/{serviceNodeExtensionsModel.Name}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };
            if (serviceNodeExtensionsModel.IsEnabledHealthCheck)
            {
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
                    HTTP = $"{serviceNodeExtensionsModel.Protocol}://{serviceNodeExtensionsModel.IP}:{serviceNodeExtensionsModel.Port}/health",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                };
                registration.Checks = new[] { httpCheck };
            }
            consulClient.Agent.ServiceRegister(registration).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册
            });
            return app;

        }
    }
}
