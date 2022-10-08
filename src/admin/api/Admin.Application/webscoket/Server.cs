using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
namespace Magicodes.Admin.webscoket
{
    /// <summary>
    /// Socket 服务端
    /// </summary>
    public class Server
    {
        private ConnectionCollection connections;
        public ConnectionCollection Connections
        {
            get { return connections; }
            set { connections = value; }
        }
        private TcpListener listener;
        private Thread listenningthread;
        public Server(TcpListener listener)
        {
            this.connections = new ConnectionCollection();
            this.listener = listener;
        }
        public void Start()
        {
            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    this.connections.Add(new Connection(stream));
                }
            }
        }
        /// <summary>
        /// 服务器端侦听消息
        /// </summary>
        public void Listenning()
        {
            while (true)
            {
                Thread.Sleep(200);
                foreach (Connection connection in this.connections)
                {
                    if (connection.NetworkStream.CanRead && connection.NetworkStream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int count = connection.NetworkStream.Read(buffer, 0, buffer.Length);
                        Console.Write("================Server 服务器接受到的信息==================" + SocketFactoryAppService.encoding.GetString(buffer, 0, count));
                    }
                }
            }
        }
        /// <summary>
        /// 启动服务器监听
        /// </summary>
        public void StartListen()
        {
            listenningthread = new Thread(new ThreadStart(Listenning));
            listenningthread.Start();
        }
    }
}