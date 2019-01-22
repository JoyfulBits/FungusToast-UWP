using System.Net.Http;

namespace FungusToastApiClientLibrary.Serialization
{
    public interface ISerializer
    {
        StringContent SerializeToHttpStringContent(object objectToSerialize);
        T DeserializeObject<T>(string serializedObject);
    }
}