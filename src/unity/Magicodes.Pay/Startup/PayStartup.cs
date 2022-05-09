// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayStartup.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Threading;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Configuration;
using Magicodes.Alipay;
using Magicodes.Alipay.Builder;
using Magicodes.Alipay.Global;
using Magicodes.Alipay.Global.Builder;
using Magicodes.Pay.Log;
using Magicodes.Pay.PaymentCallbacks;
using Magicodes.Pay.WeChat;
using Magicodes.Pay.WeChat.Builder;
using Magicodes.PayNotify.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Magicodes.Alipay.Global.Dto;
using Magicodes.Pay.WeChat.Config;

namespace Magicodes.Pay.Startup
{
    public class PayStartup
    {
        /// <summary>
        ///     配置支付
        /// </summary>
        public static async Task ConfigAsync(ILogger logger, IIocManager iocManager, IConfigurationRoot config, ISettingManager settingManager)
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
            }

            #region 支付配置

            await WeChatPayConfig(LogAction, iocManager, config);
            await AlipayConfig(LogAction, iocManager, config);
            await GlobalAlipayConfig(LogAction, iocManager, config);

            PayNotifyConfig(LogAction, iocManager);

            #endregion
        }

        /// <summary>
        /// 支付宝支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="iocManager"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static async Task AlipayConfig(Action<string, string> logAction, IIocManager iocManager, IConfigurationRoot config)
        {
            AlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    using (var settingManagerObj = iocManager.ResolveAsDisposable<ISettingManager>())
                    {
                        var settingManager = settingManagerObj.Object;

                        AlipaySettings alipaySettings = null;
                        if (Convert.ToBoolean(settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.IsActive).Result))
                        {
                            alipaySettings = new AlipaySettings
                            {
                                AlipayPublicKey = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AlipayPublicKey).Result,
                                AppId = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AppId).Result,
                                Uid = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Uid).Result,
                                PrivateKey = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.PrivateKey).Result,
                            };

                            var charSet = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.CharSet).Result;
                            if (!charSet.IsNullOrWhiteSpace())
                            {
                                alipaySettings.CharSet = charSet;
                            }
                            var gatewayurl = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Gatewayurl).Result;
                            if (!gatewayurl.IsNullOrWhiteSpace())
                            {
                                alipaySettings.Gatewayurl = gatewayurl;
                            }
                            var notify = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Notify).Result;
                            if (!notify.IsNullOrWhiteSpace())
                            {
                                alipaySettings.Notify = notify;
                            }
                            var signType = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.SignType).Result;
                            if (!signType.IsNullOrWhiteSpace())
                            {
                                alipaySettings.SignType = signType;
                            }
                        }
                        else if (!config["Alipay:IsEnabled"].IsNullOrWhiteSpace() && Convert.ToBoolean(config["Alipay:IsEnabled"]))
                        {
                            alipaySettings = new AlipaySettings
                            {
                                AlipayPublicKey = config["Alipay:PublicKey"],
                                AppId = config["Alipay:AppId"],
                                Uid = config["Alipay:Uid"],
                                PrivateKey = config["Alipay:PrivateKey"]
                            };
                            if (!config["Alipay:CharSet"].IsNullOrWhiteSpace())
                            {
                                alipaySettings.CharSet = config["Alipay:CharSet"];
                            }
                            if (!config["Alipay:Gatewayurl"].IsNullOrWhiteSpace())
                            {
                                alipaySettings.Gatewayurl = config["Alipay:Gatewayurl"];
                            }
                            if (!config["Alipay:Notify"].IsNullOrWhiteSpace())
                            {
                                alipaySettings.Notify = config["Alipay:Notify"];
                            }
                            if (!config["Alipay:SignType"].IsNullOrWhiteSpace())
                            {
                                alipaySettings.SignType = config["Alipay:SignType"];
                            }
                        }

                        return alipaySettings;
                    }
                }).Build();

            //注册支付宝支付API
            if (!iocManager.IsRegistered<IAlipayAppService>())
                iocManager.Register<IAlipayAppService, AlipayAppService>(DependencyLifeStyle.Transient);

        }

        /// <summary>
        /// 国际支付宝支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="iocManager"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static async Task GlobalAlipayConfig(Action<string, string> logAction, IIocManager iocManager, IConfigurationRoot config)
        {
            #region 支付宝支付
            {
                GlobalAlipayBuilder.Create()
                    .WithLoggerAction(logAction)
                    .RegisterGetPayConfigFunc(() =>
                    {
                        using (var settingManagerObj = iocManager.ResolveAsDisposable<ISettingManager>())
                        {
                            var settingManager = settingManagerObj.Object;

                            IGlobalAlipaySettings alipaySettings = null;
                            if (Convert.ToBoolean(settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.IsActive).Result))
                            {
                                alipaySettings = new GlobalAlipaySettings
                                {
                                    Key = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Key).Result,
                                    Partner = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Partner).Result,
                                    Gatewayurl = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Gatewayurl).Result,
                                    Notify = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Notify).Result,
                                    ReturnUrl = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.ReturnUrl).Result,
                                    Currency = settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.Currency).Result,
                                };
                                var splitFundSettingsString =
                                    settingManager.GetSettingValueAsync(AppSettings.GlobalAliPayManagement.SplitFundSettings).Result;
                                if (!splitFundSettingsString.IsNullOrWhiteSpace())
                                {
                                    alipaySettings.SplitFundInfo = JsonConvert.DeserializeObject<List<SplitFundSettingInfoDto>>(splitFundSettingsString);
                                }
                            }
                            else if (!config["GlobalAlipay:IsEnabled"].IsNullOrWhiteSpace() && Convert.ToBoolean(config["Alipay:IsEnabled"]))
                            {
                                alipaySettings = new GlobalAlipaySettings
                                {
                                    Key = config["GlobalAlipay:Key"],
                                    Partner = config["GlobalAlipay:Partner"],
                                    Gatewayurl = config["GlobalAlipay:Gatewayurl"],
                                    Notify = config["GlobalAlipay:Notify"],
                                    ReturnUrl = config["GlobalAlipay:ReturnUrl"],
                                    Currency = config["GlobalAlipay:Currency"],
                                };
                                var splitFundSettingsString = config["GlobalAlipay:SplitFundInfo"];
                                if (!splitFundSettingsString.IsNullOrWhiteSpace())
                                {
                                    alipaySettings.SplitFundInfo = JsonConvert.DeserializeObject<List<SplitFundSettingInfoDto>>(splitFundSettingsString);
                                }

                            }

                            return alipaySettings;
                        }
                    }).Build();

                //注册支付宝支付API
                if (!iocManager.IsRegistered<IGlobalAlipayAppService>())
                    iocManager.Register<IGlobalAlipayAppService, GlobalAlipayAppService>(DependencyLifeStyle.Transient);
            }
            #endregion
        }

        /// <summary>
        /// 微信支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="iocManager"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static async Task WeChatPayConfig(Action<string, string> logAction, IIocManager iocManager, IConfigurationRoot config)
        {
            //微信支付配置
            WeChatPayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    using (var settingManagerObj = iocManager.ResolveAsDisposable<ISettingManager>())
                    {
                        var settingManager = settingManagerObj.Object;
                        DefaultWeChatPayConfig weChatPayConfig = null;

                        if (Convert.ToBoolean(settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.IsActive).Result))
                        {
                            weChatPayConfig = new DefaultWeChatPayConfig()
                            {
                                PayAppId = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.AppId).Result,
                                MchId = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.MchId).Result,
                                PayNotifyUrl = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.PayNotifyUrl).Result,
                                TenPayKey = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.TenPayKey).Result,
                            };
                        }
                        else if (!config["WeChat:Pay:IsEnabled"].IsNullOrWhiteSpace() && Convert.ToBoolean(config["WeChat:Pay:IsEnabled"]))
                        {
                            weChatPayConfig = new DefaultWeChatPayConfig
                            {
                                MchId = config["WeChat:Pay:MchId"],
                                PayNotifyUrl = config["WeChat:Pay:NotifyUrl"],
                                TenPayKey = config["WeChat:Pay:TenPayKey"],
                                PayAppId = config["WeChat:Pay:AppId"]
                            };
                        }

                        return weChatPayConfig;
                    }
                }).Build();

            //注册微信支付API
            if (!iocManager.IsRegistered<WeChatPayApi>())
            {
                iocManager.Register<WeChatPayApi>(DependencyLifeStyle.Transient);
            }
        }

        /// <summary>
        /// 支付回调配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <param name="iocManager"></param>
        private static void PayNotifyConfig(Action<string, string> logAction, IIocManager iocManager)
        {
            void PayAction(string key, string outTradeNo, string transactionId, int totalFee, JObject data)
            {
                using (var paymentCallbackManagerObj = iocManager.ResolveAsDisposable<PaymentCallbackManager>())
                {
                    var paymentCallbackManager = paymentCallbackManagerObj?.Object;
                    if (paymentCallbackManager == null)
                    {
                        throw new ApplicationException("支付回调管理器异常，无法执行回调！");
                    }
                    AsyncHelper.RunSync(async () => await paymentCallbackManager.ExecuteCallback(key, outTradeNo, transactionId, totalFee, data));
                }
            }
            
            //支付回调设置
            PayNotifyBuilder
                .Create()
                //设置日志记录
                .WithLoggerAction(logAction).WithPayNotifyFunc(input =>
                {
                    using (var unitOfWorkManagerObj = iocManager.ResolveAsDisposable<IUnitOfWorkManager>())
                    {
                        using (var uow = unitOfWorkManagerObj.Object.Begin())
                        {
                            switch (input.Provider)
                            {
                                case "wechat":
                                    {
                                        using (var obj = iocManager.ResolveAsDisposable<WeChatPayApi>())
                                        {
                                            var api = obj.Object;
                                            return api.PayNotifyHandler(input.Request.Body, (output, error) =>
                                            {
                                                //获取微信支付自定义数据
                                                if (string.IsNullOrWhiteSpace(output.Attach))
                                                {
                                                    throw new UserFriendlyException("自定义参数不允许为空！");
                                                }

                                                var data = JsonConvert.DeserializeObject<JObject>(output.Attach);
                                                var key = data["key"]?.ToString();
                                                var outTradeNo = output.OutTradeNo;
                                                var totalFee = int.Parse(output.TotalFee);
                                                PayAction(key, outTradeNo, output.TransactionId, totalFee, data);
                                                uow.Complete();
                                            });
                                        }

                                    }
                                case "alipay":
                                    {
                                        using (var obj = iocManager.ResolveAsDisposable<IAlipayAppService>())
                                        {
                                            var api = obj.Object;

                                            var dictionary = input.Request.Form.ToDictionary(p => p.Key, p2 => p2.Value.FirstOrDefault()?.ToString());
                                            //签名校验
                                            if (!api.PayNotifyHandler(dictionary))
                                            {
                                                throw new UserFriendlyException("支付宝支付签名错误！");
                                            }
                                            var outTradeNo = input.Request.Form["out_trade_no"];
                                            var tradeNo = input.Request.Form["trade_no"];
                                            var charset = input.Request.Form["charset"];
                                            var totalFee = (int)(decimal.Parse(input.Request.Form["total_fee"]) * 100);
                                            var businessParams = input.Request.Form["business_params"];
                                            if (string.IsNullOrWhiteSpace(businessParams))
                                            {
                                                throw new UserFriendlyException("自定义参数不允许为空！");
                                            }
                                            var data = JsonConvert.DeserializeObject<JObject>(businessParams);
                                            var key = data["key"]?.ToString();
                                            PayAction(key, outTradeNo, tradeNo, totalFee, data);
                                            uow.Complete();
                                            return Task.FromResult("success");
                                        }
                                    }
                                //国际支付宝
                                case "global.alipay":
                                    {
                                        using (var obj = iocManager.ResolveAsDisposable<IGlobalAlipayAppService>())
                                        {
                                            var api = obj.Object;

                                            var dictionary = input.Request.Form.ToDictionary(p => p.Key, p2 => p2.Value.FirstOrDefault()?.ToString());
                                            //签名校验
                                            if (!api.PayNotifyHandler(dictionary))
                                            {
                                                throw new UserFriendlyException("支付宝支付签名错误！");
                                            }
                                            var outTradeNo = input.Request.Form["out_trade_no"];
                                            var tradeNo = input.Request.Form["trade_no"];
                                            var charset = input.Request.Form["charset"];
                                            var totalFee = (int)(Convert.ToDecimal(input.Request.Form["total_fee"]) * 100);
                                            //交易状态
                                            string tradeStatus = input.Request.Form["trade_status"];
                                            using (var transactionLogHelperObj = iocManager.ResolveAsDisposable<TransactionLogHelper>())
                                            {
                                                var customData = transactionLogHelperObj.Object.GetCustomDataByOutTradeNo(outTradeNo);
                                                if (string.IsNullOrWhiteSpace(customData))
                                                {
                                                    throw new UserFriendlyException("自定义参数不允许为空！");
                                                }
                                                var data = JsonConvert.DeserializeObject<JObject>(customData);
                                                var key = data["key"]?.ToString();
                                                PayAction(key, outTradeNo, tradeNo, totalFee, data);
                                            }
                                            uow.Complete();
                                            return Task.FromResult("success");
                                        }
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                    return null;
                }).Build();
        }
    }
}