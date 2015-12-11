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
            string myTestString = "biteme";    
            byte[] stringPacket = packet.Serialize(myTestString);
            Console.WriteLine(Encoding.Default.GetString(stringPacket));
            Console.WriteLine(stringPacket.Length + " bytes");

            // create an object payload
            Person p = new Person("Cena", "John");
            SimpleTCP.Packet<Person> packet1 = new SimpleTCP.Packet<Person>();
            byte[] personPacket = packet1.Serialize(p);
            Console.WriteLine(Encoding.Default.GetString(personPacket));
            Console.WriteLine(stringPacket.Length + " bytes");

            // get IP addresses and ports
            IPAddress[] MyAddresses = GetHostNameAndIP();
            string myAddress = MyAddresses[1].ToString();
            ushort defaultPort = 8080;

            // create asynchronous client
            AsynchronousClient.StartClient(MyAddresses[0], defaultPort, stringPacket);
            AsynchronousClient.StartClient(MyAddresses[0], defaultPort, personPacket);

            // create raw socket and pass in payloads
            RawSocketClient.SendPacket(myAddress, myAddress, defaultPort, defaultPort, stringPacket);
            RawSocketClient.SendPacket(myAddress, myAddress, defaultPort, defaultPort, personPacket);

            // create asynchronous client
            AsynchronousClient.StartClient(MyAddresses[0], defaultPort, stringPacket);
            AsynchronousClient.StartClient(MyAddresses[0], defaultPort, personPacket);

            // create TCPHeader object (nonsense)
            TCPHeader t = new TCPHeader(8080, 8080, 1, 1, 0, 1, 512, 0, 0);
            byte[] tcpHeader = t.getHeader();
            Console.WriteLine(tcpHeader.Length + " bytes");
            string s = System.Text.Encoding.UTF8.GetString(tcpHeader, 0, tcpHeader.Length);
            Console.WriteLine(s);

            // test with raw
            
            /*using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP))
            {
                sock.Bind(new IPEndPoint(MyAddresses[0], 8080));
                byte[] data = stringPacket;
                sock.SendTo(data, new IPEndPoint(MyAddresses[0], 8080));
            };*/

            // test with TCP client
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 51000;
                string server = MyAddresses[0].ToString();
                TcpClient tcpClient = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = stringPacket;

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = tcpClient.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", Encoding.Default.GetString(stringPacket));

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                tcpClient.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        } 

        public IPAddress[] GetHostNameAndIP()
        {
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr;
            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
            }
            Console.ReadLine();
        }
    }
}
