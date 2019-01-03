using Website.Core.Interfaces.Models;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents a claim that a user possesses. 
    /// </summary>
    public class UserClaim : IUserClaim
    {
        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue { get; set; }

        /// <summary>
        /// Navigation property for the User of this claim.
        /// </summary>
        public virtual User User { get; set; }
    }
}
