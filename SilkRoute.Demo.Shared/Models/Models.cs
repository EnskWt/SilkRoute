using Microsoft.AspNetCore.Http;

namespace SilkRoute.Demo.Shared.Models
{
    public sealed record ComplexDto(int Id, string Name);

    public sealed record NestedDto(int Id, ComplexDto Inner);

    public sealed record EchoResponse(string Case, object? Payload = null);

    public sealed record DtoWithNestedBytes(int Id, string Name, byte[] Data);

    public sealed record DtoWithNestedStream(int Id, string Name, Stream Data);

    public sealed record DtoWithNestedFormData(int Id, string Name, IFormFile File);
}
