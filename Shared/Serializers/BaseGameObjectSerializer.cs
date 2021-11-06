using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace UnityMultiplayer.Shared.Networking.Serializers
{
    public abstract class BaseGameObjectSerializer : MonoBehaviour
    {
        public abstract byte[] Serialize(DatagramHolder datagramHolder);
        public abstract DatagramHolder Deserialize(byte[] bytes);
    }
}
