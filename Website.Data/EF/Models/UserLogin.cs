using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    public class UserLogin : UserLoginKey
    {
        /// <summary>
        /// Gets or sets the friendly name used in a UI for this login.
        /// </summary>
        public string ProviderDisplayName { get; set; }
        /// <summary>
        /// Gets or sets the primary key of the user associated with this login.
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Composite key of a login.
    /// </summary>
    public class UserLoginKey
    {
        /// <summary>
        /// Gets or sets the login provider for the login (e.g. facebook, google)
        /// </summary>
        public string LoginProvider { get; set; }
        /// <summary>
        /// Gets or sets the unique provider identifier for this login.
        /// </summary>
        public string ProviderKey { get; set; }
    }
}
