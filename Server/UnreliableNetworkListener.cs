using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;
using UnityMultiplayer.Shared.Networking.Serializers;

namespace UnityMultiplayer.Server
{
    public class UnreliableNetworkListener
    {
        private readonly BaseGameObjectSerializer _serializer;
        private Dictionary<IPEndPoint, NetworkChannel> _networkChannels;

        public UnreliableNetworkListener(IPEndPoint local,
            Dictionary<IPEndPoint, NetworkChannel> networkChannels,
            BaseGameObjectSerializer serializer)
        {
            UdpClient = new UdpClient(local);
            _networkChannels = networkChannels;
            _serializer = serializer;

            // This is set to ignore ICMP packets that might be sent after the server
            // send a datagram to a client whose udp socket was already closed.
            // Google WSAECONNRESET for more info.
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            UdpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        }

        public UdpClient UdpClient { get; }

        public void ReadIntoChannels()
        {
            IPEndPoint remote = null;
            while (UdpClient.Available > 0)
            {
                byte[] messageBytes = UdpClient.Receive(ref remote);
                DatagramHolder datagramHolder = _serializer.Deserialize(messageBytes);
                _networkChannels[remote].UnreliableChannel.VirtuallyFillMessageQueue(datagramHolder);
            }
        }
    }
}
