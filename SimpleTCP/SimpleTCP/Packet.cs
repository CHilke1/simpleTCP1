using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleTCP
{
    [Serializable]
    public class Packet<T>
    {
        public byte[] Serialize(T obj)
        {
            Object o = obj;
            MemoryStream ms = new MemoryStream(); //packet size will be maximum of 4KB.
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            ms.Close();
            return ms.ToArray();
        }

        //Deserializing the List
        public T Deserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream();//packet size will be maximum of 4KB.
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }
            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();   
            Object o = bf.Deserialize(ms);
            T obj = (T)o;
            ms.Close();
            return obj;
        }
    }
    public enum PacketType
    {
        initType = 0, //it's nothing on this code.
        login
    }

    
    /*public class Packet
    {
        public int Length;
        public int Type;

        public Packet()
        {
            this.Length = 0;
            this.Type = 0;
        }
        public byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4); //packet size will be maximum of 4KB.
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
        public Object Deserialize(byte[] bt)
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

    }*/
}
