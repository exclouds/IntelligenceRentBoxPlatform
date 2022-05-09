using Abp.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Builder;
using Magicodes.WeChat.Startup;
using System;

namespace Magicodes.WeChat
{
    [DependsOn(
    typeof(AdminCoreModule))]
    public class WeChatModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WeChatModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var settingManager = IocManager.Resolve<ISettingManager>();
            var appConfiguration = IocManager.Resolve<IAppConfigurationAccessor>().Configuration;
            var cacheManager = IocManager.Resolve<ICacheManager>();

            WeChatStartup.Config(Logger, IocManager, appConfiguration, settingManager, cacheManager);       
        }
    }
}
