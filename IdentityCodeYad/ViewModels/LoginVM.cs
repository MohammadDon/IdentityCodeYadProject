using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace IdentityCodeYad.ViewModels;

public class LoginVM
{
    [Required]
    [Display(Name = "User Name")]
    [StringLength(200)]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Remember Me?")]
    public bool RememberMe { get; set; }
}

public class LoginWithPhoneVM
{
    [Required,Phone]
    public string Phone { get; set; }

    public string ReturnUrl { get; set; }
    public List<AuthenticationScheme>? ExternalLogins { get; set; }
}