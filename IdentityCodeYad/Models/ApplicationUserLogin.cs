using Microsoft.AspNetCore.Identity;

namespace IdentityCodeYad.Models;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; }
}