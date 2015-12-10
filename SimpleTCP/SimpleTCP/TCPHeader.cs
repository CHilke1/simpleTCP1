using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

#region
/*                                    
    0                   1                   2                   3   
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |          Source Port          |       Destination Port        |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                        Sequence Number                        |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                    Acknowledgment Number                      |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |  Data |           |U|A|P|R|S|F|                               |
   | Offset| Reserved  |R|C|S|S|Y|I|            Window             |
   |       |           |G|K|H|T|N|N|                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |           Checksum            |         Urgent Pointer        |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                    Options                    |    Padding    |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                             data                              |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                            TCP Header Format
Source Port: 16 bits

   The source port number.

Destination Port: 16 bits

   The destination port number.

Sequence Number: 32 bits

   The sequence number of the first data octet in this segment (except
   when SYN is present). If SYN is present the sequence number is the
   initial sequence number (ISN) and the first data octet is ISN+1.

Acknowledgment Number: 32 bits

   If the ACK control bit is set this field contains the value of the
    next sequence number the sender of the segment is expecting to
    receive.Once a connection is established this is always sent.

Data Offset: 4 bits

 The number of 32 bit words in the TCP Header.This indicates where
the data begins.  The TCP header(even one including options) is an
    integral number of 32 bits long.

Reserved: 6 bits

   Reserved for future use.  Must be zero.

Control Bits: 6 bits(from left to right):

   URG:  Urgent Pointer field significant
    ACK:  Acknowledgment field significant
    PSH:  Push Function
    RST:  Reset the connection
    SYN:  Synchronize sequence numbers
    FIN:  No more data from sender

Window: 16 bits

   The number of data octets beginning with the one indicated in the
    acknowledgment field which the sender of this segment is willing to
    accept.

Checksum: 16 bits

   The checksum field is the 16 bit one's complement of the one's
    complement sum of all 16 bit words in the header and text.If a
    segment contains an odd number of header and text octets to be
    checksummed, the last octet is padded on the right with zeros to
    form a 16 bit word for checksum purposes.  The pad is not
    transmitted as part of the segment.While computing the checksum,
   the checksum field itself is replaced with zeros.

   The checksum also covers a 96 bit pseudo header conceptually
    prefixed to the TCP header.This pseudo header contains the Source
    Address, the Destination Address, the Protocol, and TCP length.
   This gives the TCP protection against misrouted segments.This
    information is carried in the Internet Protocol and is transferred
    across the TCP / Network interface in the arguments or results of
     calls by the TCP on the IP.
 
                     +--------+--------+--------+--------+
                     |           Source Address          |
                     +--------+--------+--------+--------+
                     |         Destination Address       |
                     +--------+--------+--------+--------+
                     |  zero  |  PTCL  |    TCP Length   |
                     +--------+--------+--------+--------+

       The TCP Length is the TCP header length plus the data length in
      octets (this is not an explicitly transmitted quantity, but is
      computed), and it does not count the 12 octets of the pseudo
      header.

Urgent Pointer: 16 bits

    This field communicates the current value of the urgent pointer as a
    positive offset from the sequence number in this segment.The
    urgent pointer points to the sequence number of the octet following
    the urgent data.This field is only be interpreted in segments with
    the URG control bit set.

Options: variable

    Options may occupy space at the end of the TCP header and are a
    multiple of 8 bits in length.All options are included in the
    checksum.  An option may begin on any octet boundary.  There are two
    cases for the format of an option:

      Case 1:  A single octet of option-kind.

      Case 2:  An octet of option-kind, an octet of option-length, and
               the actual option-data octets.

    The option-length counts the two octets of option-kind and
    option-length as well as the option-data octets.

    Note that the list of options may be shorter than the data offset
    field might imply.  The content of the header beyond the
    End-of-Option option must be header padding (i.e., zero).

    A TCP must implement all options.
    Currently defined options include (kind indicated in octal):

      Kind Length Meaning
      ----     ------    -------
       0         -       End of option list.
       1         -       No-Operation.
       2         4       Maximum Segment Size.
      

    Specific Option Definitions

      End of Option List

        +--------+
        |00000000|
        +--------+
         Kind=0

        This option code indicates the end of the option list.  This
        might not coincide with the end of the TCP header according to
        the Data Offset field.  This is used at the end of all options,
        not the end of each option, and need only be used if the end of
        the options would not otherwise coincide with the end of the TCP
        header.

      No-Operation

        +--------+
        |00000001|
        +--------+
         Kind=1

        This option code may be used between options, for example, to
        align the beginning of a subsequent option on a word boundary.
        There is no guarantee that senders will use this option, so
        receivers must be prepared to process options even if they do
        not begin on a word boundary.

      Maximum Segment Size

        +--------+--------+---------+--------+
        |00000010|00000100|   max seg size   |
        +--------+--------+---------+--------+
         Kind=2   Length=4

        Maximum Segment Size Option Data:  16 bits

          If this option is present, then it communicates the maximum
          receive segment size at the TCP which sends this segment.
          This field must only be sent in the initial connection request
          (i.e., in segments with the SYN control bit set).  If this
          option is not used, any segment size is allowed.

Padding: variable

    The TCP header padding is used to ensure that the TCP header ends
    and data begins on a 32 bit boundary.  The padding is composed of
    zeros.
*/

# endregion
// http://www.freesoft.org/CIE/Course/Section4/8.htm

namespace SimpleTCP
{
    public class TCPHeader
    {
        ushort source_port; // 16 bits
        ushort destination_port; // 16 bits
        uint sequence_number; // 32 bits
        uint acknowledgment_number; // 32 bits
        ushort data_offset_reserved_control_bits;
        //BitArray data_offset = new BitArray(4); // 4 bits
        //BitArray reserved = new BitArray(6); // 6 zeroes
        //BitArray control_bits = new BitArray(6); // 6 bits
        ushort window; // 16 bits
        ushort checksum; // 16 bits
        ushort urgent_pointer; // 16 bits
        uint options; // variable
        //480 bits max - 60 bytes

        public TCPHeader(ushort source_port, ushort destination_port, ushort sequence_number, uint acknowledgment_number,
            ushort data_offset_reserved_control_bits, ushort window, ushort checksum, ushort urgent_pointer, uint options)
        {
            setSourcePort(source_port);
            setDestinationPort(destination_port);
            setSequenceNumber(sequence_number);
            setAcknowledgmentNumber(acknowledgment_number);
            setDataOffsetReservedControlBits(data_offset_reserved_control_bits);
            setWindow(window);
            setChecksum(checksum);
            setUrgentPointer(urgent_pointer);
            setOptions(options);
        }

        public byte[] getHeader()
        {
            byte[] sp = getSourcePort();
            byte[] dp = getDestinationPort();
            byte[] sn = getSequenceNumber();
            byte[] an = getAcknowledgmentNumber();
            byte[] w = getWindow();
            byte[] cs = getCheckSum();
            byte[] up = getUrgentPointer();
            byte[] o = getOptions();
            byte[] headerArray = Concat(sp, dp, sn, an, w, cs, up, o);
            return headerArray;
        }

        private void setSourcePort(ushort source_port = 80)
        {
            this.source_port = source_port;
        }

        private byte[] getSourcePort()
        {
            return ConvertToByteArray(source_port);
        }

        private void setDestinationPort(ushort destination_port = 80)
        {
            this.destination_port = destination_port;
        }

        private byte[] getDestinationPort()
        {
            return ConvertToByteArray(destination_port);
        }

        private void setSequenceNumber(uint seq_num)
        {
            sequence_number = seq_num;
        }
        private byte[] getSequenceNumber()
        {
            return ConvertToByteArray(sequence_number);
        }

        private void setAcknowledgmentNumber(uint ack_num)
        {
            acknowledgment_number = ack_num;
        }
        private byte[] getAcknowledgmentNumber()
        {
            return ConvertToByteArray(acknowledgment_number);
        }

        private void setDataOffsetReservedControlBits(ushort data_offset_reserved_control_bits)
        {
            this.data_offset_reserved_control_bits = data_offset_reserved_control_bits;
        }

        private ushort getDataOffsetReservedControlBits()
        {
            return data_offset_reserved_control_bits;
        }

        private void setWindow(ushort window)
        {
            this.window = window; 
        }
        private byte[] getWindow()
        {
            return ConvertToByteArray(window);
        }

        private void setChecksum(ushort checksum)
        {
            this.checksum = checksum;
        }
        private byte[] getCheckSum()
        {
            return ConvertToByteArray(checksum);
        }

        public void setUrgentPointer(ushort urgent_pointer)
        {
            this.urgent_pointer = urgent_pointer;
        }

        public byte[] getUrgentPointer()
        {
            return ConvertToByteArray(urgent_pointer);
        }
        public void setOptions(uint options)
        {
            this.options = options;
        }
        public byte[] getOptions()
        {
            return ConvertToByteArray(options);
        }
        private byte[] ConvertToByteArray(uint value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);
            return byteArray;
        }
        private byte[] ConvertToByteArray(ushort value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);
            return byteArray;
        }

        private  int ZeroBit(int value, int position)
        {
            return value & ~(1 << position);
        }
        public static byte[] Concat(params byte[][] arrays)
        {
            using (var mem = new MemoryStream(arrays.Sum(a => a.Length)))
            {
                foreach (var array in arrays)
                {
                    mem.Write(array, 0, array.Length);
                }
                return mem.ToArray();
            }
        }

        //public getString()
        //{
        //    string s = System.Text.Encoding.UTF8.GetString(this);
        //    return s;
        //}

        /*public T FromByteArray<T>(byte[] rawValue)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public byte[] ToByteArray(Object obj, int maxLength)
        {
            int rawsize = Marshal.SizeOf(obj);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            handle.Free();
            if (maxLength < rawdata.Length)
            {
                byte[] temp = new byte[maxLength];
                Array.Copy(rawdata, temp, maxLength);
                return temp;
            }
            else
            {
                return rawdata;
            }
        }*/
    }

    public enum control_digits
    {
        URG,
        ACK,
        PSH,
        RST,
        SYN,
        FIN,
    }

}
