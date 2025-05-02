namespace SilkRoute.Demo.Shared.Models.Dto;

public sealed class DtoWithNestedBytes
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
}