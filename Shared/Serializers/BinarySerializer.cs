using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace UnityMultiplayer.Shared.Networking.Serializers
{
    class BinarySerializer : BaseGameObjectSerializer
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public override byte[] Serialize(DatagramHolder datagramHolder)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, datagramHolder);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public override DatagramHolder Deserialize(byte[] datagramHolderBytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(datagramHolderBytes, 0, datagramHolderBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (DatagramHolder)binaryFormatter.Deserialize(ms);
            }
        }
    }
}
