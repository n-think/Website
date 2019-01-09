using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    public class UserLogin : IdentityUserLogin<int>
    {
        public virtual User User { get; set; }
    }
}
