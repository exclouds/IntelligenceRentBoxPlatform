using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace Magicodes.Admin.webscoket
{
    public class Connection
    {
        private NetworkStream networkStream;
        public NetworkStream NetworkStream
        {
            get { return networkStream; }
            set { networkStream = value; }
        }
        private string connectionName;
        public string ConnectionName
        {
            get { return connectionName; }
            set { connectionName = value; }
        }
        public Connection(NetworkStream networkStream, string connectionName)
        {
            this.networkStream = networkStream;
            this.connectionName = connectionName;
        }
        public Connection(NetworkStream networkStream) : this(networkStream, string.Empty) { }
    }
}