using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleTCP
{
    class TCPDemo
    {
        string Uri = "localhost";
        public void Client(Object obj)
        {
            using (TcpClient client = new TcpClient (Uri, 51111))
            using (NetworkStream n = client.GetStream())
            {
                BinaryWriter writer = new BinaryWriter (n);
                writer.Write("Hello");
                writer.Flush();
                Console.WriteLine(new BinaryReader(n).ReadString());
            }
        }

        public void Server()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 51111);
            listener.Start();
            using (TcpClient c = listener.AcceptTcpClient())
            using (NetworkStream n = c.GetStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                n.Seek(0, SeekOrigin.Begin);
                object objectType = formatter.Deserialize(n);
                n.Flush();
            }
            listener.Stop();
        }
    }
}
