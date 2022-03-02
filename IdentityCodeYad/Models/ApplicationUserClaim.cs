using Microsoft.AspNetCore.Identity;

namespace IdentityCodeYad.Models;

public class ApplicationUserClaim : IdentityUserClaim<string>
{
    public virtual ApplicationUser User { get; set; }
}