using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents the link between a user and a role.
    /// </summary>
    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
