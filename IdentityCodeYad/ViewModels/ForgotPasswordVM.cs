using System.ComponentModel.DataAnnotations;

namespace IdentityCodeYad.ViewModels;

public class ForgotPasswordVM
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}