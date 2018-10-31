using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents the link between a user and a role.
    /// </summary>
    public class UserRole : IUserRole
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        public virtual string RoleId { get; set; }

        /// <summary>
        /// Nav property for User
        /// </summary>
        public virtual User User { get; set; }
        /// <summary>
        /// Nav property for Role
        /// </summary>
        public virtual Role Role { get; set; }
    }
}
