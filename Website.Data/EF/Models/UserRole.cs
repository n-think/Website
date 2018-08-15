using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents the link between a user and a role.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        public virtual string RoleId { get; set; }
    }
}
