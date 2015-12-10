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
    public class SimpleTCP<T>
    {
        Object obj;
        public SimpleTCP(T obj)
        {
            this.obj = obj;
        }
        
        public void ToBinary()
        {
            System.Console.WriteLine("{0}", obj.ToString());
            MemoryStream stream = SerializeToStream(obj);
            T newObj = (T)DeserializeFromStream(stream);
            System.Console.WriteLine("{0}", newObj.ToString());
        }

        private static MemoryStream SerializeToStream(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream;
        }

        private static object DeserializeFromStream(MemoryStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        public byte[] ReadData (Stream stream)
        {
            byte[] fileData = new BinaryReader(stream).ReadBytes((int)stream.Length);
            return fileData;
        }

        public void SaveData (Stream stream)
        {

        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
