using System.Net.Http;
using FungusToastApiClient.Models;

namespace FungusToastApiClient.Serialization
{
    public interface ISerializer
    {
        StringContent SerializeToHttpStringContent(object objectToSerialize);
        T DeserializeObject<T>(string serializedObject);
    }
}