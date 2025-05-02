namespace SilkRoute.Demo.Shared.Models.Dto;

public sealed class DtoWithNestedStream
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Stream Data { get; set; }
}