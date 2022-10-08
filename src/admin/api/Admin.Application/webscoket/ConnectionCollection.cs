using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Magicodes.Admin.webscoket
{
    public class ConnectionCollection : CollectionBase
    {
        public ConnectionCollection() { }
        public void Add(Connection conn)
        {
            List.Add(conn);
        }
        public Connection this[int index]
        {
            get
            {
                return List[index] as Connection;
            }
            set
            {
                List[index] = value;
            }
        }
        public Connection this[string connectionName]
        {
            get
            {
                foreach (Connection connection in List)
                {
                    if (connection.ConnectionName == connectionName)
                        return connection;
                }
                return null;
            }
        }
    }
}