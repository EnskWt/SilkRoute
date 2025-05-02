using Microsoft.AspNetCore.Http;

namespace SilkRoute.Demo.Shared.Models.Dto;

public sealed class DtoWithNestedFormData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IFormFile File { get; set; }
}