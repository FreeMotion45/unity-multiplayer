using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UnityMultiplayer.Shared.Networking.SecureConnection
{
    public class ReliableNetworkMessager
    {
        public void WriteMessage(TcpClient client, byte[] message)
        {
            if (client.Connected)
            {
                byte[] fullMessageBuffer = ConstructMessage(message);
                client.GetStream().Write(fullMessageBuffer, 0, fullMessageBuffer.Length);
            }
        }

        public void TaskedWriteMessage(TcpClient client, byte[] message)
        {
            if (client.Connected)
            {
                byte[] fullMessageBuffer = ConstructMessage(message);
                Task.Run(async () =>
                {
                    await client.GetStream().WriteAsync(fullMessageBuffer, 0, fullMessageBuffer.Length);
                });
            }
        }

        public List<byte[]> ReadAvailableMessages(TcpClient client)
        {
            List<byte[]> dataReceived = new List<byte[]>();
            byte[] lengthBytes = new byte[4];
            if (client.Connected)
            {
                while (client.Available > 0)
                {
                    NetworkStream network = client.GetStream();
                    network.Read(lengthBytes, 0, lengthBytes.Length);
                    int messageLength = BitConverter.ToInt32(lengthBytes, 0);
                    byte[] messageBytes = new byte[messageLength];
                    network.Read(messageBytes, 0, messageLength);
                    dataReceived.Add(messageBytes);
                }
            }
            return dataReceived;
        }

        private byte[] ConstructMessage(byte[] message)
        {
            byte[] fullMessageBuffer = new byte[4 + message.Length];
            byte[] lengthBytes = BitConverter.GetBytes(message.Length);
            lengthBytes.CopyTo(fullMessageBuffer, 0);
            message.CopyTo(fullMessageBuffer, 4);
            return fullMessageBuffer;
        }
    }
}
