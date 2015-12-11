using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleTCP
{
    interface IPacketItem
    {
        byte[] ToArray();
    }

    class Packet<THeader, TContent>
        where THeader : IPacketItem
        where TContent : IPacketItem
    {
        // header and content must be initialized somehow;
        // maybe, using existing instances, maybe using new() constraint
        public THeader Header { get; private set; }
        public TContent Content { get; private set; }

        public byte[] ToArray(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4); //packet size will be maximum of 4KB.
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
    }
}
