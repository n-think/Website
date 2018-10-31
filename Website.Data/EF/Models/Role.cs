using System.Collections.Generic;
using Website.Data.ModelsInterfaces;

namespace Website.Data.EF.Models
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    public class Role : IRole
    {

        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }


        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public virtual string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; }


        /// <summary>
        /// Navigation property for the Users this role belongs to.
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Returns the name of the role.
        /// </summary>
        /// <returns>The name of the role.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
