using Assets.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Shared.Networking.Serializers
{
    class ProtoSerializer
    {
        public string[] DeserializeCommandArray(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public string ExtractType(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] SerializeCommandArray(IEnumerable<BaseCommand> commands)
        {
            throw new NotImplementedException();
        }
    }
}
