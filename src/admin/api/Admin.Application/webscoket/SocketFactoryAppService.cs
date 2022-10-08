using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.webscoket
{
    public class SocketFactoryAppService : AdminAppServiceBase
    {
        private Thread serverListenThread;
        public static Encoding encoding = Encoding.GetEncoding("utf-8");
        /// <summary>
        /// 启动服务端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        [HttpGet]
        public void StartServer(string ip, int port)
        {
            IPAddress ipa = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(ipa, port);
            listener.Start();
            Server server = new Server(listener);
            serverListenThread = new Thread(new ThreadStart(server.Start));
            serverListenThread.Start();
            server.StartListen();
        }
        /// <summary>
        /// 检测端口是否被打开
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>true未打开</returns>
        public bool SocketCheck(string ip, int port)
        {
            Socket sock = null;

            try
            {
                IPAddress ipa = IPAddress.Parse(ip);
                IPEndPoint point = new IPEndPoint(ipa, port);
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(point);
                return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
            finally
            {
                if (sock != null)
                {
                    sock.Close();
                    sock.Dispose();
                }
            }
        }
        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [HttpGet]
        public Connection StartClient(string ip, int port)
        {
            return Client.StartClient(IPAddress.Parse(ip), port);
        }
        /// <summary>
        /// 发消息给服务器端
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        public static void SendMessage(string message, Connection connection)
        {
            byte[] buffer = encoding.GetBytes(message);
            connection.NetworkStream.Write(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 获取服务器端返回的消息
        /// </summary>
        /// <param name="connection"></param>
        public static void GetMessage(Connection connection)
        {
            // connection.NetworkStream.Length;
            byte[] buffer = new byte[1024];
            connection.NetworkStream.Write(buffer, 0, buffer.Length);
        }
    }
}
