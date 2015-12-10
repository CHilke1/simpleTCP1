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
            SimpleTCP.TCPHeader h = new SimpleTCP.TCPHeader(80, 80, 90, 100,50,100, 10,10, 100);
            byte[] packetHeader = h.getHeader();
            Console.WriteLine(packetHeader.Length + " bytes");
            string s = System.Text.Encoding.UTF8.GetString(packetHeader, 0, packetHeader.Length);
            Console.WriteLine(s);
            Console.Read();
        }
    }
}
