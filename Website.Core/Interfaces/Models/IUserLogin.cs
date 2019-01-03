namespace Website.Core.Interfaces.Models
{
    public interface IUserLogin
    {
        /// <summary>
        /// Gets or sets the friendly name used in a UI for this login.
        /// </summary>
        string ProviderDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user associated with this login.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the login provider for the login (e.g. facebook, google)
        /// </summary>
        string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the unique provider identifier for this login.
        /// </summary>
        string ProviderKey { get; set; }
    }
}