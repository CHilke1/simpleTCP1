using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Tester t = new Tester();
            t.Run();
        }
    }

    class Tester
    {
        public void Run()
        {
            SimpleTCP.TCPHeader TCPHeader = new SimpleTCP.TCPHeader(80, 80, 90, 100,50,100, 10,10, 100);
            SimpleTCP.Ipv4Header IPHeader = new SimpleTCP.Ipv4Header();
            SimpleTCP.Packet p = new SimpleTCP.Packet();
            byte[] packetHeader = TCPHeader.getHeader();
            Console.WriteLine(packetHeader.Length + " bytes");
            Console.WriteLine(IPHeader.Length);
            string s = System.Text.Encoding.UTF8.GetString(packetHeader, 0, packetHeader.Length);
            Console.WriteLine(s);
            Console.Read();
        }
    }
}
