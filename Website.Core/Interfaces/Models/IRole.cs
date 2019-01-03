namespace Website.Core.Interfaces.Models
{
    public interface IRole
    {
        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        string ConcurrencyStamp { get; set; }
    }
}