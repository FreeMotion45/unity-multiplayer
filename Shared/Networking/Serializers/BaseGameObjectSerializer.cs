using Assets.Shared.Commands;
using Assets.Shared.Networking.Datagrams;
using Assets.Shared.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Shared.Networking.Serializers
{
    public abstract class BaseGameObjectSerializer : MonoBehaviour
    {
        public abstract byte[] Serialize(DatagramHolder datagramHolder);
        public abstract DatagramHolder Deserialize(byte[] bytes);
    }
}
