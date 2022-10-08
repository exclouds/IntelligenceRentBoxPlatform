using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace Magicodes.Admin.webscoket
{
    /// <summary>
    /// Socket 客户端
    /// </summary>
    public class Client
    {
        // 超时时间，毫秒
        public const int CONNECT_TIMEOUT = 10;
        public Client() { }
        /// <summary>
        /// 启动Socket客户端
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Connection StartClient(IPAddress ipaddress, int port)
        {
            TcpClient client = new TcpClient();
            client.SendTimeout = CONNECT_TIMEOUT;
            client.ReceiveTimeout = CONNECT_TIMEOUT;
            client.Connect(ipaddress, port);
            Connection connection = new Connection(client.GetStream());
            return connection;
        }
    }
}