using System.ComponentModel.DataAnnotations;

namespace IdentityCodeYad.ViewModels;

public class RegisterVM
{
    [Required]
    [MaxLength(250)]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [Phone]
    public string Phone { get; set; }
    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string RePassword { get; set; }
}