using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SilkRoute.Demo.Shared.Models
{
    public sealed record ComplexDto(int Id, string Name);

    public sealed record NestedDto(int Id, ComplexDto Inner);

    public sealed record DtoWithNestedBytes(int Id, string Name, byte[] Data);

    public sealed record DtoWithNestedStream(int Id, string Name, Stream Data);

    public sealed record DtoWithNestedFormData(int Id, string Name, IFormFile File);

    public sealed class RequestSnapshot
    {
        public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

        public string HttpMethod { get; init; } = "";
        public string Path { get; init; } = "";
        public string? RoutePattern { get; init; }

        public Dictionary<string, string?> RouteValues { get; init; } = new();
        public Dictionary<string, string?> Query { get; init; } = new();
        public Dictionary<string, string?> Headers { get; init; } = new();

        public BodySnapshot? Body { get; init; }
        public FormSnapshot? Form { get; init; }

        public sealed class BodySnapshot
        {
            public string? ContentType { get; init; }
            public long? Length { get; init; }

            public string Kind { get; init; } = "unknown";

            public string? Text { get; init; }

            public string? Base64 { get; init; }
            public int? BytesLength { get; init; }
        }

        public sealed class FormSnapshot
        {
            public Dictionary<string, string?> Fields { get; init; } = new();
            public List<FormFileSnapshot> Files { get; init; } = new();
        }

        public sealed class FormFileSnapshot
        {
            public string Name { get; init; } = "";
            public string FileName { get; init; } = "";
            public string? ContentType { get; init; }
            public long Length { get; init; }
        }

        public static string ToJson(object? obj)
            => JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }
}
