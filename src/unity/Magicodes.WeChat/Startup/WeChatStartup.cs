using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.Token;
using Magicodes.WeChat.SDK.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Magicodes.WeChat.Startup
{
    public class WeChatStartup
    {
        /// <summary>
        /// 配置公众号
        /// </summary>
        public static void Config(ILogger logger, IIocManager iocManager, IConfigurationRoot config, ISettingManager settingManager, ICacheManager cacheManager)
        {
            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                {
                    logger.Error(message);
                }
                else
                {
                    logger.Debug(message);
                }
            };

            //appsettings中获取配置
            var configInfo = new WeChatConfig
            {
                AppId = config["Authentication:WeChat:AppId"],
                AppSecret = config["Authentication:WeChat:AppSecret"],
                WeiXinAccount = config["Authentication:WeChat:WeiXinAccount"],
                Token = config["ToKen"]
            };
            //如果启用了配置管理则从数据库中得到配置
            if (Convert.ToBoolean(settingManager.GetSettingValue(AppSettings.WeChatManagement.IsEnabled)))
            {
                configInfo.AppId = settingManager.GetSettingValue(AppSettings.WeChatManagement.AppId);
                configInfo.AppSecret = settingManager.GetSettingValue(AppSettings.WeChatManagement.AppSecret);
                configInfo.WeiXinAccount = settingManager.GetSettingValue(AppSettings.WeChatManagement.WeiXinAccount);
                configInfo.Token = settingManager.GetSettingValue(AppSettings.WeChatManagement.Token);
            }

            WeChatSDKBuilder.Create()
                .WithLoggerAction(LogAction)
                .Register(WeChatFrameworkFuncTypes.GetKey,
                    model =>
                    {
                        //使用租户id作为key，来确保不同租户加载不同的公众号配置
                        var key = iocManager.Resolve<IAbpSession>()?.TenantId;
                        if (key == null) return "0";

                        return key.ToString();
                    })
                .Register(WeChatFrameworkFuncTypes.Config_GetWeChatConfigByKey, model => configInfo)
                .Register(WeChatFrameworkFuncTypes.APIFunc_GetAccessToken,
                    model => {
                        //1)通过配置拿到key
                        //用租户id创建redis的key
                        var key = $"{AdminConsts.WeChatAccessTokenRedisKey}{iocManager.Resolve<IAbpSession>()?.TenantId??0}";

                        //2）通过key从缓存获取AccessToken的一个Json
                        string tokenResultJson = cacheManager.GetCache(AdminConsts.WeChatAccessTokenJsonKey).GetOrDefault(key)?.ToString();

                        //3）如果获取成功，则直接返回
                        if (tokenResultJson == null)
                        {
                            //4）如果获取为空，则通过GetByCustomConfig API获取Token
                            var tokenResult = WeChatApisContext.Current.TokenApi.GetByCustomConfig(configInfo);

                            if (!tokenResult.IsSuccess())
                                throw new ApiArgumentException("获取接口访问凭据失败：" + tokenResult.GetFriendlyMessage() + "（" + tokenResult.DetailResult + "）");

                            tokenResultJson = JsonConvert.SerializeObject(tokenResult);

                            //5）获取成功写入缓存，缓存时间小于Token过期时间
                            cacheManager.GetCache(AdminConsts.WeChatAccessTokenJsonKey).Set(key, tokenResultJson, tokenResult.ExpiresTime - DateTime.Now);
                            return tokenResult;
                        }
                        return JsonConvert.DeserializeObject(tokenResultJson) as TokenApiResult;
                    })
                .Build();
        }
    }
}
