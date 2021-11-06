using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Shared.Networking.Datagrams
{
    [Serializable]
    public class DatagramHolder
    {
        public DatagramHolder(DatagramType datagramType, object data)
        {
            DatagramType = datagramType;
            Data = data;
        }

        public DatagramType DatagramType { get; private set; }
        public object Data { get; private set; }
    }
}
