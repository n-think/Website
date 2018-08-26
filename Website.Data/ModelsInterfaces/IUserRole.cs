namespace Website.Data.ModelsInterfaces
{
    public interface IUserRole
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        string RoleId { get; set; }
    }
}