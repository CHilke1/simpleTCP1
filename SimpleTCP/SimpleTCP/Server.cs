using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace SimpleTCP
{
    class Server
    {
        static void CheckforMessages()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 51111);
            listener.Start();
            using (TcpClient c = listener.AcceptTcpClient())
            using (NetworkStream n = c.GetStream())
            {
                Object T = new BinaryReader(n).ReadString();
                BinaryWriter w = new BinaryWriter(n);
                w.Write(T.ToString() + " received");
                w.Flush();
            }
            listener.Stop();
        }
    }
}
