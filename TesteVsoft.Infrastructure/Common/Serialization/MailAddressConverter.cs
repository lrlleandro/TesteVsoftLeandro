using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TesteVsoft.Infrastructure.Common.Serialization;

public class MailAddressConverter : JsonConverter<MailAddress?>
{
    public override MailAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var emailString = reader.GetString();
            if (string.IsNullOrWhiteSpace(emailString))
            {
                return null;
            }
            return new MailAddress(emailString);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.TryGetProperty("address", out JsonElement addressElement))
                {
                    var emailString = addressElement.GetString();
                    if (string.IsNullOrWhiteSpace(emailString))
                    {
                        return null;
                    }
                    return new MailAddress(emailString);
                }
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, MailAddress? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Address);
        }
    }
}