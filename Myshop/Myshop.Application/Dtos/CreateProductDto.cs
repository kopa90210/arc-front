
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Add this line

namespace Myshop.Application.Dtos
{
public class ProductCreateDtoV
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public int Stock { get; set; }
    public IFormFile? Image { get; set; }
}
}
