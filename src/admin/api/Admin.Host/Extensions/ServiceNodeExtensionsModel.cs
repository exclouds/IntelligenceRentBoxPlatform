using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Extensions
{
    /// <summary>
    /// 健康检查扩展模型
    /// </summary>
    public class ServiceNodeExtensionsModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 协议（http|https）
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// 是否开启健康检查
        /// </summary>
        public bool IsEnabledHealthCheck { get; set; }
    }
}
