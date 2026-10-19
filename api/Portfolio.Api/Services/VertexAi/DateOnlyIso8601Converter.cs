using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfolio.Api.Services.VertexAi;

internal sealed class DateOnlyIso8601Converter : JsonConverter<DateOnly>
{
    private const string RoundTripFormat = "O";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateOnly.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToString(RoundTripFormat));
}
