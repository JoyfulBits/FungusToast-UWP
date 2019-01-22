using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FungusToastApiClientLibrary.Serialization
{
    public class Serializer : ISerializer
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public StringContent SerializeToHttpStringContent(object objectToSerialize)
        {
            var jsonObject = JsonConvert.SerializeObject(objectToSerialize, _serializerSettings);
            return new StringContent(jsonObject, Encoding.UTF8, "application/json");
        }

        public T DeserializeObject<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject, _serializerSettings);
        }
    }
}