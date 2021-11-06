using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace UnityMultiplayer.Shared.Networking.UnreliableConnection
{
    public class UnreliableMessage
    {
        public UnreliableMessage(UnreliableNetworkClient sender, DatagramHolder data)
        {
            Sender = sender;
            Data = data;
        }

        public UnreliableNetworkClient Sender { get; }
        public DatagramHolder Data { get; }
    }
}
