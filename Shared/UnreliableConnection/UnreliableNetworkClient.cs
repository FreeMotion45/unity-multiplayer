using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assets.Shared.Commands;
using UnityEngine;
using Assets.Shared.Networking.Serializers;
using Assets.Shared.Player;
using Assets.Shared.Networking.Datagrams;

namespace Assets.Shared.Networking
{
    public class UnreliableNetworkClient
    {
        private const int KEEP_ALIVE_INTERVAL = 240; // MS
        private UdpClient _udpClient;
        private BaseGameObjectSerializer _serializer;
        private IPEndPoint _remote;
        private readonly Queue<DatagramHolder> _insertedMessages;
        private DateTime _lastKeepAliveReceived;
        private DateTime _lastKeepAliveSent;
        private Queue<byte[]> _messagesWaitingForKeepAlive = new Queue<byte[]>();

        public UnreliableNetworkClient(IPEndPoint remoteEndpoint, BaseGameObjectSerializer serializer, IPEndPoint local = null)
        {
            _remote = remoteEndpoint;
            _serializer = serializer;
            if (local == null)
            {
                _udpClient = new UdpClient();
            }
            else
            {
                _udpClient = new UdpClient(local);
            }
        }

        public UnreliableNetworkClient(IPEndPoint remoteEndpoint, BaseGameObjectSerializer serializer, UdpClient udpClient)
        {
            _remote = remoteEndpoint;
            _serializer = serializer;
            _udpClient = udpClient;
            _insertedMessages = new Queue<DatagramHolder>();
        }

        public UnreliableNetworkClient(string ip, int port, BaseGameObjectSerializer serializer)
            : this(new IPEndPoint(IPAddress.Parse(ip), port), serializer)
        {
        }

        public bool ThisInitiatedConnection { get; set; }
        public bool IsConnected { get; private set; }

        public void Connect()
        {
            if (ThisInitiatedConnection)
            {
                _udpClient.Connect(_remote);
            }
            IsConnected = true;
            _lastKeepAliveReceived = DateTime.Now;
            _lastKeepAliveSent = DateTime.Now;
            SendDatagramHolder(new DatagramHolder(DatagramType.UnreliableKeepAlive, null));
        }

        public void Disconnect()
        {
            IsConnected = false;
            if (ThisInitiatedConnection)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }

        public void VirtuallyFillMessageQueue(DatagramHolder datagramHolder)
        {
            if (ThisInitiatedConnection)
            {
                throw new Exception("Can't fill queue if this is the one who initiated the connection.");
            }
            _insertedMessages.Enqueue(datagramHolder);
        }

        public void SendDatagramHolder(DatagramHolder datagramHolder)
        {
            if (IsConnected)
            {
                Send(_serializer.Serialize(datagramHolder));
            }
        }

        public DatagramHolder[] ReceiveAllMessages()
        {
            if (!IsConnected) return new DatagramHolder[0];

            List<DatagramHolder> datagramHolders = new List<DatagramHolder>();
            if (ThisInitiatedConnection)
            {
                IPEndPoint sender = _remote;
                while (_udpClient.Available > 0)
                {
                    try
                    {
                        byte[] datagram = _udpClient.Receive(ref sender);
                        DatagramHolder dgram = _serializer.Deserialize(datagram);
                        if (dgram.DatagramType == DatagramType.UnreliableKeepAlive)
                        {                            
                            _lastKeepAliveReceived = DateTime.Now;
                            continue;
                        }
                        datagramHolders.Add(dgram);
                    }
                    catch (SocketException se)
                    {
                        if (se.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            // WSACONNRESET, remote port is not listening. This means
                            // the server crashed.
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            else
            {
                while (_insertedMessages.Count > 0)
                {
                    DatagramHolder dgram = _insertedMessages.Dequeue();
                    if (dgram.DatagramType == DatagramType.UnreliableKeepAlive)
                    {                        
                        _lastKeepAliveReceived = DateTime.Now;
                        continue;
                    }
                    datagramHolders.Add(dgram);
                }
            }
            HandleKeepAlive();
            return datagramHolders.ToArray();
        }

        private void Send(byte[] bytes)
        {
            if (!IsConnected) return;

            _messagesWaitingForKeepAlive.Enqueue(bytes);

            // * 1.5 to compensate for network lags.
            if (MillisecondsSinceLastReceivedKeepAlive() >= KEEP_ALIVE_INTERVAL * 1.5)
            {
                return;
            }

            while (_messagesWaitingForKeepAlive.Count > 0)
            {
                GeneralSend(_messagesWaitingForKeepAlive.Dequeue());
            }
        }

        private void GeneralSend(byte[] bytes)
        {
            if (ThisInitiatedConnection)
            {
                _udpClient.Send(bytes, bytes.Length);
            }
            else
            {
                _udpClient.Send(bytes, bytes.Length, _remote);
            }
        }

        private void HandleKeepAlive()
        {
            // TODO: Make this run in a task.
            double millisecondsSinceLastReceived = MillisecondsSinceLastReceivedKeepAlive();
            double millisecondsSinceLastSent = (DateTime.Now - _lastKeepAliveSent).TotalMilliseconds;
            if (millisecondsSinceLastReceived >= KEEP_ALIVE_INTERVAL * 3)
            {
                Debug.Log($"No response from unreliable connection after {KEEP_ALIVE_INTERVAL * 3} MS. Terminating...");
                Disconnect();
            }
            else if (millisecondsSinceLastSent >= KEEP_ALIVE_INTERVAL * 0.9)
            {
                SendKeepAlive();                
                _lastKeepAliveSent = DateTime.Now;
            }
        }

        private void SendKeepAlive()
        {
            // Bypassing the keep alive message queue
            byte[] bytes = _serializer.Serialize(new DatagramHolder(DatagramType.UnreliableKeepAlive, null));
            GeneralSend(bytes);
        }

        private double MillisecondsSinceLastReceivedKeepAlive()
        {
            return (DateTime.Now - _lastKeepAliveReceived).TotalMilliseconds;
        }
    }
}
