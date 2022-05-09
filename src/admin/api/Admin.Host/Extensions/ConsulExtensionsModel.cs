using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Extensions
{
    /// <summary>
    /// 服务发现扩展模型
    /// </summary>
    public class ConsulExtensionsModel
    {
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
    }
}
