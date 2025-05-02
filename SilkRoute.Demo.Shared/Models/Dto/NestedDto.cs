namespace SilkRoute.Demo.Shared.Models.Dto;

public sealed class NestedDto
{
    public int Id { get; set; }
    public ComplexDto Inner { get; set; }
}