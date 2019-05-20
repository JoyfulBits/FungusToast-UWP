using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApiClient.Models
{
    public class JoinGameResult
    {
        public enum JoinGameResultType
        {
            JoinedNotStarted = 1,
            JoinedStarted = 2,
            GameFull = 3,
            UserAlreadyInGame = 4
        }

        //TODO can't figure out how to deserialize the integer back into the enum so just making an int for now
        public int ResultType { get; set; }
        [JsonIgnore]
        public JoinGameResultType ResultTypeEnumValue => (JoinGameResultType) ResultType;
    }

    public class EnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
