using Assets.Shared.Networking.CommandHandling.DatagramHandlers;
using Assets.Shared.Networking.Datagrams;
using Assets.Shared.Networking.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMultiplayer.Shared.Networking.Datagrams.Handling
{
    public class DatagramHandlerResolver : MonoBehaviour
    {
        private Dictionary<DatagramType, BaseDatagramHandler> _typeHandlingMap;

        private void Start()
        {
            _typeHandlingMap = new Dictionary<DatagramType, BaseDatagramHandler>();
        }

        public void AddHandler(DatagramType type, BaseDatagramHandler handler)
        {
            _typeHandlingMap[type] = handler;
        }

        public void RemoveHandler(DatagramType datagramType)
        {
            _typeHandlingMap.Remove(datagramType);
        }

        public BaseDatagramHandler Resolve(DatagramType type)
        {
            if (!_typeHandlingMap.ContainsKey(type))
            {
                Debug.LogError("Received datagram of type " + type + " which is not mapped in the resolver. Ignoring it.");
            }
            return _typeHandlingMap[type];
        }
    }
}
