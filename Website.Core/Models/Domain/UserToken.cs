using Website.Core.Interfaces.Models;

namespace Website.Core.Models.Domain
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class UserToken : UserTokenKey, IUserToken
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Navigation property for the User of this Token.
        /// </summary>
        public virtual User User { get; set; }
    }

    /// <summary>
    /// Composite key of a token.
    /// </summary>
    public class UserTokenKey
    {
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public string LoginProvider { get; set; }
        public string Name { get; set; }
    }
}
