using Assets.Shared.Networking.Datagrams;
using Assets.Shared.Networking.SecureConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnityMultiplayer.Shared.Networking
{
    public class NetworkChannel
    {
        public NetworkChannel(ReliableNetworkClient reliableChannel, UnreliableNetworkClient unreliableChannel)
        {
            ReliableChannel = reliableChannel;
            UnreliableChannel = unreliableChannel;
        }

        public ReliableNetworkClient ReliableChannel { get; private set; }
        public UnreliableNetworkClient UnreliableChannel { get; private set; }
        public IPEndPoint RemoteEndPoint { get => (IPEndPoint)ReliableChannel.Client.Client.RemoteEndPoint; }

        public DatagramHolder[] GetAllReliableAndUnreliableMessages()
        {
            DatagramHolder[] receivedReliables = ReliableChannel.ReadAvailableMessages();
            DatagramHolder[] receivedUnreliables = UnreliableChannel.ReceiveAllMessages();

            List<DatagramHolder> datagramHolders = receivedReliables.ToList();
            datagramHolders.AddRange(receivedUnreliables);
            return datagramHolders.ToArray();
        }

        public void Connect()
        {
            ReliableChannel.Connect();
            UnreliableChannel.Connect();
        }

        public void Close()
        {
            ReliableChannel.Disconnect();
            ReliableChannel = null;

            UnreliableChannel.Disconnect();
            UnreliableChannel = null;
        }
    }
}
