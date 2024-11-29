using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.Utils
{
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int x = 0, y = 0;

            // Read JSON object
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value.ToString();
                    reader.Read(); // Move to property value

                    if (propertyName == "x")
                        x = Convert.ToInt32(reader.Value);
                    else if (propertyName == "y")
                        y = Convert.ToInt32(reader.Value);
                }
            }

            return new Vector2Int(x, y);
        }
    }
}