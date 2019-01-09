using Microsoft.AspNetCore.Identity;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents a claim that is granted to all users within a role.
    /// </summary>
    public class RoleClaim : IdentityRoleClaim<int>
    {

    }
}
