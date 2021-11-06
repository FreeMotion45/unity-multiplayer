using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMultiplayer.Shared.Networking.Datagrams
{
    public enum DatagramType
    {
        Command,
        WorldState,
        ErrorCorrection,
        Events,
        SenderChatMessage,
        ReceiverChatMessage,
        UnreliableKeepAlive,
        PlayerJoined,
        JoinRequest,
    }
}
