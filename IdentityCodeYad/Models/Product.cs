using System.ComponentModel.DataAnnotations;

namespace IdentityCodeYad.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int Price { get; set; }
}