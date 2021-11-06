using Assets.Shared.Networking.Datagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMultiplayer.Shared.Networking.Datagrams.Handling
{
    public abstract class BaseDatagramHandler : MonoBehaviour
    {
        [SerializeField] private DatagramHandlerResolver _datagramHandlerResolver;
        [SerializeField] private DatagramType _datagramType;

        protected virtual void Start()
        {
            _datagramHandlerResolver.AddHandler(_datagramType, this);
        }

        public abstract void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel);
    }
}
