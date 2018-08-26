namespace Website.Data.EF.Models
{
    public interface IUserToken
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        string UserId { get; set; }

        string LoginProvider { get; set; }
        string Name { get; set; }
    }
}