using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class UserToken : UserTokenKey
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Composite key of a token.
    /// </summary>
    public class UserTokenKey
    {
        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
    }
}
