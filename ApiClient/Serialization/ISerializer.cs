using System.Net.Http;

namespace ApiClient.Serialization
{
    public interface ISerializer
    {
        StringContent SerializeToHttpStringContent(object objectToSerialize);
        T DeserializeObject<T>(string serializedObject);
    }
}