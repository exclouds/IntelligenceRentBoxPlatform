using Magicodes.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Magicodes.Admin.webscoket
{
    public class ChatAppService : AdminAppServiceBase
    {

        public void StartServer()
        {
            //// 服务端  
            //SocketFactory factory = new SocketFactory();
            //factory.StartServer("192.168.70.71", 3001);

            //// 客户端  
            //Connection conn = factory.StartClient(IPAddress.Parse("192.168.70.71"), 3001);
            //SocketFactory.SendMessage("我的测试信息", conn);
        }

    }
}
