using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Packet

{
    public enum PacketType
    {

        initType = 0, //it's nothing on this code.
        login
    }

    [Serializable]
    public class Packet
    {
        public int Length;
        public int Type;

        public Packet()
        {

            this.Length = 0;
            this.Type = 0;


        }

        public static byte[] Serialize(Object o)
        {

            MemoryStream ms = new MemoryStream(1024 * 4); //packet size will be maximum of 4KB.
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();

        }

        public static Object Desirialize(byte[] bt)
        {

            MemoryStream ms = new MemoryStream(1024 * 4);//packet size will be maximum of 4KB.

            foreach (byte b in bt)
            {

                ms.WriteByte(b);

            }

            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;

        }
    }
}//end of Packet.cs