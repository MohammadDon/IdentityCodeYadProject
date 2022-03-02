using Microsoft.AspNetCore.Identity;

namespace IdentityCodeYad.Models;

public class ApplicationUserToken : IdentityUserToken<string>
{
    public virtual ApplicationUser User { get; set; }
}