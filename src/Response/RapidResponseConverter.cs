using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RapidAPISDK.Response
{
    internal class RapidResponseConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!TryReadUntilProperty(reader, "outcome"))
                throw new Exception("Invalid response from server. outcome missing.");

            var outcome = reader.Value.ToString();

            if (!TryReadUntilProperty(reader, "payload"))
                throw new Exception("Invalid response from server. payload missing.");

            object res;
            if (outcome == "error")
                res = Activator.CreateInstance(objectType, ReadError(reader, serializer));
            else
                res = Activator.CreateInstance(objectType, ReadPayload(reader, objectType, serializer));

            ReadToEnd(reader);
            return res;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(RapidResponse<>);
        }

        #endregion

        #region Private Methods

        private object ReadPayload(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            var payloadType = objectType.GetGenericArguments()[0];

            //Handle if payload is returned as string and not as part of the json
            if (reader.ValueType == typeof(string) && !payloadType.IsPrimitive)
            {
                using (var stringReader = new StringReader(reader.Value.ToString()))
                {
                    return serializer.Deserialize(stringReader, payloadType);
                }
            }

            return serializer.Deserialize(reader, payloadType);
        }

        private RapidAPIServerException ReadError(JsonReader reader, JsonSerializer serializer)
        {
            var error = serializer.Deserialize<object>(reader);
            return new RapidAPIServerException(error);
        }

        private bool TryReadUntilProperty(JsonReader reader, string propertyName)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(propertyName))
                    return reader.Read();
            }

            return false;
        }

        private void ReadToEnd(JsonReader reader)
        {
            while (reader.Read())
            {
            }
        }

        #endregion

    }
}
