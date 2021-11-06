using Assets.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Assets.Shared.Player;
using UnityEngine;
using Assets.Shared.Networking.Datagrams;

namespace Assets.Shared.Networking.Serializers
{
    class JsonSerializer
    {
        private static int c = 0;
        private static List<JObject> sent = new List<JObject>();

        public byte[] SerializeCommandArray(IEnumerable<BaseCommand> commands, PlayerController playerController)
        {
            JObject jObject = new JObject();
            string[] commandTypes = commands.Select(c => c.CommandType).ToArray();
            jObject["InnerDatagram"] = new JArray(commandTypes);
            jObject["Rotation"] = new JObject();
            //Debug.Log(playerController.transform.position);
            jObject["Position"] = new JObject();
            jObject["PositionBefore"] = new JObject();
            jObject["Position"]["X"] = playerController.transform.position.x;
            jObject["Position"]["Y"] = playerController.transform.position.y;
            jObject["Position"]["Z"] = playerController.transform.position.z;
            jObject["Forward"] = new JObject();
            jObject["Forward"]["X"] = playerController.transform.forward.x;
            jObject["Forward"]["Z"] = playerController.transform.forward.z;
            jObject["Counter"] = c;
            
            jObject["Type"] = "CommandArray";
            sent.Add(jObject);
            return Encoding.UTF8.GetBytes(jObject.ToString());
        }

        public string[] DeserializeCommandArray(byte[] bytes)
        {
            JObject jObject = JObject.Parse(Encoding.UTF8.GetString(bytes));
            return jObject["InnerDatagram"].ToObject<string[]>();
        }

        public string ExtractType(byte[] bytes)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(bytes));
            return jObject["Type"].ToObject<string>();
        }

        public byte[] Serialize(DatagramHolder datagramHolder)
        {
            throw new NotImplementedException();
        }

        public DatagramHolder Deserialize(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
