using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.SecureConnection;
using UnityMultiplayer.Shared.Networking.Serializers;

namespace UnityMultiplayer.Server
{
    class ReliableNetworkListener
    {        
        private readonly TcpListener _listener;
        private readonly List<ReliableNetworkClient> _clients;
        private readonly ReliableNetworkMessager _messageReader;
        private readonly BaseGameObjectSerializer _serializer;        

        public ReliableNetworkListener(IPEndPoint localEndpoint,
            ReliableNetworkMessager secureConnectionManager,
            BaseGameObjectSerializer serializer)
        {
            _listener = new TcpListener(localEndpoint);
            _messageReader = secureConnectionManager;
            _clients = new List<ReliableNetworkClient>();
            _serializer = serializer;
        }

        public void Start()
        {
            _listener.Start();
        }

        public void CloseListener()
        {
            foreach (ReliableNetworkClient client in _clients)
            {
                if (client.Client.Connected)
                {
                    client.Disconnect();
                }                
            }

            _listener.Stop();            
            Debug.Log("TCP Listener stopped.");
        }

        public ReliableNetworkClient[] AcceptNewConnections()
        {
            List<ReliableNetworkClient> newClients = new List<ReliableNetworkClient>();            
            while (_listener.Pending())
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                ReliableNetworkClient secureNetworkClient = new ReliableNetworkClient(tcpClient, _messageReader, _serializer);
                newClients.Add(secureNetworkClient);
            }
            return newClients.ToArray();
        }
    }
}
