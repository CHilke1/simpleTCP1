using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace SimpleTCP
{
    public class TCPClient
    {
        public static void SendPacket(IPAddress address, Int32 destport, byte[] packet) { 
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = destport;
                TcpClient tcpClient = new TcpClient(address.ToString(), port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = packet;

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = tcpClient.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", Encoding.Default.GetString(packet));

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
            Console.ReadKey();
        }
    }
}
