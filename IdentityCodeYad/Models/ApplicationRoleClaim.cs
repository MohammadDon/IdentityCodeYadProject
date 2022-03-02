using Microsoft.AspNetCore.Identity;

namespace IdentityCodeYad.Models;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public virtual ApplicationRole Role { get; set; }
}