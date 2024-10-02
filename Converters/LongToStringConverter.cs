namespace WebTestJsonConverter.Converters;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class LongToStringConverterForId : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(long);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String && IsIdField(reader.Path))
        {
            var stringValue = reader.Value.ToString();
            if (long.TryParse(stringValue, out var longValue))
            {
                return longValue;
            }

            throw new JsonSerializationException($"Invalid string value for long: {stringValue}");
        }

        return serializer.Deserialize(reader, objectType); // Default behavior for other fields
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (IsIdField(writer.Path))
        {
            var longValue = (long)value;
            writer.WriteValue(longValue.ToString());
        }
        else
        {
            writer.WriteValue(value); // Default behavior for other fields
        }
    }

    // Helper method to check if a property is an ID field
    private bool IsIdField(string propertyName)
    {
        return propertyName.EndsWith("id", StringComparison.OrdinalIgnoreCase)
               || propertyName.EndsWith("Id", StringComparison.OrdinalIgnoreCase);
    }
}