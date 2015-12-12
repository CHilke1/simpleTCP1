using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using SimpleTCP;

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
            // create string payload with IP address
            SimpleTCP.Packet<string> packet = new SimpleTCP.Packet<string>();
            string myTestString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            byte[] stringPacket = packet.Serialize(myTestString);
            Console.WriteLine("creating string packet\n");
            Console.WriteLine(Encoding.Default.GetString(stringPacket));
            Console.WriteLine(stringPacket.Length + " bytes");

            // create an object payload
            Person p = new Person("Cena", "John");
            SimpleTCP.Packet<Person> packet1 = new SimpleTCP.Packet<Person>();
            byte[] personPacket = packet1.Serialize(p);
            Console.WriteLine("creating object packet\n");
            Console.WriteLine(Encoding.Default.GetString(personPacket));
            Console.WriteLine(stringPacket.Length + " bytes");
            Person p2 = packet1.Deserialize(personPacket);
            Console.WriteLine(p2.ToString());

            // get IP addresses and ports
            IPAddress[] MyAddresses = GetIPAddresses();
            string myAddress = MyAddresses[1].ToString();
            ushort defaultPort = 51111;

            // create raw socket and pass in payloads
            RawSocketClient.SendPacket(myAddress, myAddress, defaultPort, defaultPort, stringPacket);
            RawSocketClient.SendPacket(myAddress, myAddress, defaultPort, defaultPort, personPacket);

            // try with TCP Network connection
            ushort TCPPort = 8080;
            TCPClient.SendPacket(MyAddresses[0], TCPPort, stringPacket);
            TCPClient.SendPacket(MyAddresses[0], TCPPort, personPacket);

            // create asynchronous client
            //AsynchronousClient.StartClient(MyAddresses[0], defaultPort, stringPacket);
            //AsynchronousClient.StartClient(MyAddresses[0], defaultPort, personPacket);

            // create TCPHeader object (nonsense)
            TCPHeader t = new TCPHeader(8080, 8080, 1, 1, 0, 1, 512, 0, 0);
            byte[] tcpHeader = t.getHeader();
            Console.WriteLine(tcpHeader.Length + " bytes");
            string s = System.Text.Encoding.UTF8.GetString(tcpHeader, 0, tcpHeader.Length);
            Console.WriteLine(s);
            Console.Read();
        }

        public IPAddress[] GetIPAddresses()
        {
            string strHostName = GetHost();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr;
        }

        public string GetHost()
        {
            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            return strHostName;
        }
    }
}
