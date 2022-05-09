using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.WeChat.SDK;

namespace Magicodes.WeChat.Startup
{
    public class WeChatConfig : IWeChatConfig
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string WeiXinAccount { get; set; }
        public string Token { get; set; }
    }
}
