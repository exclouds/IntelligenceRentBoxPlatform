using Magicodes.Admin.Configuration;
using Magicodes.Admin.webscoket;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Controllers
{
    public class SocketController
    {

        private static string ip = "";
        private static int port = 0;
        private readonly IConfigurationRoot _appConfiguration;
        public SocketController(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void StartIMServer()
        {
            SocketFactoryAppService service = new SocketFactoryAppService();
            ip = _appConfiguration["IMChat:ServerIP"].ToString();
            port = Convert.ToInt32(_appConfiguration["IMChat:ServerPort"].ToString());
            service.StartServer(ip, port);
        }
    }
}
