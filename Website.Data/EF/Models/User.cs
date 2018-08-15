using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of <see cref="User"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="User"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public User(string userName) : this()
        {
            UserName = userName;
        }

        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        public virtual string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>A value in the past means the user is not locked out.</remarks>
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// Navigation property for this user's profile.
        /// </summary>
        public virtual UserProfile UserProfile { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<UserRole> Roles { get; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<UserClaim> Claims { get; } = new List<UserClaim>();

        public virtual DateTimeOffset? LastActivityDate { get; set; }

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<UserLogin> Logins { get; } = new List<UserLogin>();

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
        {
            return this.UserName;
        }
    }
}
