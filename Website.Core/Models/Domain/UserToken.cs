using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class UserToken : IdentityUserToken<int>
    {
        public virtual User User { get; set; }
    }
}
