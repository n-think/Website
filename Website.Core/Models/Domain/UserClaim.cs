using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents a claim that a user possesses. 
    /// </summary>
    public class UserClaim : IdentityUserClaim<int>
    {
        public virtual User User { get; set; }
    }
}
