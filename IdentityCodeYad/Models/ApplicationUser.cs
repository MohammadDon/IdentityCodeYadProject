using Microsoft.AspNetCore.Identity;

namespace IdentityCodeYad.Models;

public class ApplicationUser : IdentityUser
{
    public string NickName { get; set; }
    public int Age { get; set; }

    public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}