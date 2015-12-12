using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.IO;
using System.Collections;

namespace SimpleTCP
{
    public class RawSocketClient
    {
        public static void SendPacket(string SourceAddress, string DestAddress, ushort SourcePort, ushort DestPort, byte[] Packet)
        {
            // Default/initial values, the source should be a spoofed IP :-)
            IPAddress sourceAddress = IPAddress.Parse(SourceAddress),
            destAddress = IPAddress.Parse(DestAddress),
            bindAddress = IPAddress.Any;
            ushort sourcePort = SourcePort,
            destPort = DestPort;
            int messageSize = 16,
            sendCount = 5;
            //usage();
            Console.WriteLine();
            //The correct maximum UDP message size is 65507, as determined by the following formula: 
            //0xffff - (sizeof(IP Header) +sizeof(UDP Header)) = 65535 - (20 + 8) = 65507
            //IPv4 maximum reassembly buffer size is 576, IPv6 has it at 1500.
            // Make sure parameters are consistent
            if ((sourceAddress.AddressFamily != destAddress.AddressFamily) || (sourceAddress.AddressFamily != bindAddress.AddressFamily))
            {
                Console.WriteLine("Source and destination address families don't match!");
                //usage();
                return;
            }
            // Print the command line parameters
            Console.WriteLine("Source address : {0}   \tPort: {1}", sourceAddress.ToString(), sourcePort.ToString());
            Console.WriteLine("Dest   address : {0}   \tPort: {1}", destAddress.ToString(), destPort.ToString());
            Console.WriteLine("Local interface: {0}", bindAddress.ToString());
            Console.WriteLine("Message size   : {0}", messageSize);
            Console.WriteLine("Send count     : {0}", sendCount);
            // Start building the headers
            Console.WriteLine("Building the packet header...");
            byte[] builtPacket, payLoad = Packet;
            UdpHeader udpPacket = new UdpHeader();
            ArrayList headerList = new ArrayList();

            SocketOptionLevel socketLevel = SocketOptionLevel.Udp;
            // Initialize the payload
            Console.WriteLine("Initialize the payload...");
            for (int i = 0; i < payLoad.Length; i++)
                payLoad[i] = (byte)'#';
            // Fill out the UDP header first
            Console.WriteLine("Filling out the UDP header...");
            udpPacket.SourcePort = sourcePort;
            udpPacket.DestinationPort = destPort;
            udpPacket.Length = (ushort)(UdpHeader.UdpHeaderLength + messageSize);
            udpPacket.Checksum = 0;
            if (sourceAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                Ipv4Header ipv4Packet = new Ipv4Header();
                // Build the IPv4 header
                Console.WriteLine("Building the IPv4 header...");
                ipv4Packet.Version = 4;
                ipv4Packet.Protocol = (byte)ProtocolType.Udp;
                ipv4Packet.Ttl = 2;
                ipv4Packet.Offset = 0;
                ipv4Packet.Length = (byte)Ipv4Header.Ipv4HeaderLength;
                ipv4Packet.TotalLength = (ushort)System.Convert.ToUInt16(Ipv4Header.Ipv4HeaderLength + UdpHeader.UdpHeaderLength + messageSize);
                ipv4Packet.SourceAddress = sourceAddress;
                ipv4Packet.DestinationAddress = destAddress;
                // Set the IPv4 header in the UDP header since it is required to calculate the
                //    pseudo header checksum
                Console.WriteLine("Setting the IPv4 header for pseudo header checksum...");
                udpPacket.ipv4PacketHeader = ipv4Packet;
                // Add IPv4 header to list of headers -- headers should be added in th order
                //    they appear in the packet (i.e. IP first then UDP)
                Console.WriteLine("Adding the IPv4 header to the list of header, encapsulating packet...");
                headerList.Add(ipv4Packet);
                socketLevel = SocketOptionLevel.IP;
            }
            else if (sourceAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Ipv6Header ipv6Packet = new Ipv6Header();
                // Build the IPv6 header
                Console.WriteLine("Building the IPv6 header...");
                ipv6Packet.Version = 6;
                ipv6Packet.TrafficClass = 1;
                ipv6Packet.Flow = 2;
                ipv6Packet.HopLimit = 2;
                ipv6Packet.NextHeader = (byte)ProtocolType.Udp;
                ipv6Packet.PayloadLength = (ushort)(UdpHeader.UdpHeaderLength + payLoad.Length);
                ipv6Packet.SourceAddress = sourceAddress;
                ipv6Packet.DestinationAddress = destAddress;
                // Set the IPv6 header in the UDP header since it is required to calculate the
                //    pseudo header checksum
                Console.WriteLine("Setting the IPv6 header for pseudo header checksum...");
                udpPacket.ipv6PacketHeader = ipv6Packet;
                // Add the IPv6 header to the list of headers - headers should be added in the order
                //    they appear in the packet (i.e. IP first then UDP)
                Console.WriteLine("Adding the IPv6 header to the list of header, encapsulating packet...");
                headerList.Add(ipv6Packet);
                socketLevel = SocketOptionLevel.IPv6;
            }
            // Add the UDP header to list of headers after the IP header has been added
            Console.WriteLine("Adding the UDP header to the list of header, after IP header...");
            headerList.Add(udpPacket);
            // Convert the header classes into the binary on-the-wire representation
            Console.WriteLine("Converting the header classes into the binary...");
            builtPacket = udpPacket.BuildPacket(headerList, payLoad);
            // Create the raw socket for this packet
            Console.WriteLine("Creating the raw socket using Socket()...");
            //permissions issue.
            Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Bind the socket to the interface specified
            Console.WriteLine("Binding the socket to the specified interface using Bind()...");
            rawSocket.Bind(new IPEndPoint(bindAddress, 0));
            // Set the HeaderIncluded option since we include the IP header
            Console.WriteLine("Setting the HeaderIncluded option for IP header...");
            try
            {
                rawSocket.SetSocketOption(socketLevel, SocketOptionName.HeaderIncluded, true);

            }
            catch
            {

            }
            try
            {
                // Send the packet!
                Console.WriteLine("Sending the packet...");
                for (int i = 0; i < sendCount; i++)
                {
                    int rc = rawSocket.SendTo(builtPacket, new IPEndPoint(destAddress, destPort));
                    Console.WriteLine("send {0} bytes to {1}", rc, destAddress.ToString());
                }
            }
            catch (SocketException err)
            {
                Console.WriteLine("Socket error occurred: {0}", err.Message);
                // http://msdn.microsoft.com/en-us/library/ms740668.aspx
            }
            finally
            {
                // Close the socket
                Console.WriteLine("Closing the socket...");
                rawSocket.Close();
            }
        }
        /*static void usage()
        {
            Console.WriteLine("Usage: Executable_file_name [-as source-addr] [-ad dest-addr] [-ps source-port]");
            Console.WriteLine("                  [-pd dest-port] [-x payload-size] [-n send-count] [-b bind-addr]");
            Console.WriteLine(" Options");
            Console.WriteLine("     -as source-addr     Source address for IP packet");
            Console.WriteLine("     -ad dest-addr       Destination address for IP packet");
            Console.WriteLine("     -ps source-port     Source port for UDP packet");
            Console.WriteLine("     -pd dest-port       Destination port for UDP packet");
            Console.WriteLine("     -b  bind-addr       Local address to bind raw socket to");
            Console.WriteLine("     -x  payload-size    Number of bytes for UDP payload");
            Console.WriteLine("     -n  send-count      Number of times to send packet");
            Console.WriteLine("....Else default values will be used...");
        }*/
    }
}
