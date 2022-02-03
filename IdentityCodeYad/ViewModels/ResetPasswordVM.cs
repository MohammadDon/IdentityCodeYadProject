using System.ComponentModel.DataAnnotations;

namespace IdentityCodeYad.ViewModels;

public class ResetPasswordVM
{
    [Display(Name = "New Password")]
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
    [Display(Name = "New Password")]
    [Compare(nameof(NewPassword))]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set;}
    [Required]
    public string Email { get; set; }
    [Required]
    public string Token { get; set; }
}