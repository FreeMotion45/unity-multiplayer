using Assets.Shared.Commands;
using Assets.Shared.Networking;
using Assets.Shared.Networking.CommandHandling;
using Assets.Shared.Networking.Datagrams;
using Assets.Shared.Networking.SecureConnection;
using Assets.Shared.Networking.Serializers;
using Assets.Shared.Networking.UnreliableConnection;
using Assets.Shared.Player;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Shared.Networking
{
    class PacketManager : MonoBehaviour
    {
        [SerializeField] private BaseGameObjectSerializer _serializer;
        [SerializeField] private DatagramHandlerResolver _datagramHandlerResolver;
        [SerializeField] private string _hostIP;
        [SerializeField] private int _hostPort;

        private List<NetworkChannel> _networkChannels;
        private Dictionary<IPEndPoint, NetworkChannel> _hostToChannel;        
        private IPEndPoint _localEndPoint;
        private ReliableNetworkListener _reliableNetworkListener;        
        private UnreliableNetworkListener _unreliableNetworkListener;

        public void Start()
        {            
            _networkChannels = new List<NetworkChannel>();
            _hostToChannel = new Dictionary<IPEndPoint, NetworkChannel>();            
            _localEndPoint = new IPEndPoint(IPAddress.Parse(_hostIP), _hostPort);

            _reliableNetworkListener = new ReliableNetworkListener(_localEndPoint, new ReliableNetworkMessager(), _serializer);
            _unreliableNetworkListener = new UnreliableNetworkListener(_localEndPoint, _hostToChannel, _serializer);            
            _reliableNetworkListener.Start();
        }        

        public void OnDisable()
        {
            _reliableNetworkListener.CloseListener();
            foreach (NetworkChannel channel in _networkChannels)
            {
                channel.Close();
            }
        }

        public List<NetworkChannel> CreateNewChannels()
        {
            ReliableNetworkClient[] newClients = _reliableNetworkListener.AcceptNewConnections();
            List<NetworkChannel> newChannels = new List<NetworkChannel>();
            foreach (ReliableNetworkClient client in newClients)
            {
                client.Connect();
                // Configure the UnreliableNetworkClient to send through an existing UdpClient and read unreliable messages virtually.
                IPEndPoint remote = (IPEndPoint)client.Client.Client.RemoteEndPoint;
                UnreliableNetworkClient unreliableNetworkClient = new UnreliableNetworkClient(remote, _serializer, _unreliableNetworkListener.UdpClient);                
                unreliableNetworkClient.ThisInitiatedConnection = false;
                unreliableNetworkClient.Connect();

                NetworkChannel networkChannel = new NetworkChannel(client, unreliableNetworkClient);

                _networkChannels.Add(networkChannel);
                _hostToChannel[remote] = networkChannel;
                newChannels.Add(networkChannel);
            }
            return newChannels;
        }

        public void FixedUpdate()
        {
            CreateNewChannels();
            // Unreliable connection is not really a connection.
            // We virtually insert datagrams into it by reading from the network into the channel.
            _unreliableNetworkListener.ReadIntoChannels();

            foreach (NetworkChannel networkChannel in _networkChannels)
            {
                DatagramHolder[] allMessages = networkChannel.GetAllReliableAndUnreliableMessages();
                if (!networkChannel.IsConnected)
                {
                    // ClientDataHolder.RemoveClient(networkChannel.RemoteEndPoint);
                    _hostToChannel.Remove(networkChannel.RemoteEndPoint);
                    continue;
                }
                foreach (DatagramHolder message in allMessages)
                {
                    ProcessMessage(message, networkChannel);
                }
            }
        }

        private void ProcessMessage(DatagramHolder datagramHolder, NetworkChannel sender)
        {                        
            DatagramType datagramType = datagramHolder.DatagramType;
            _datagramHandlerResolver.Resolve(datagramType).Handle(datagramHolder, sender);
        }
    }
}
