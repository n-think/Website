namespace Website.Data.ModelsInterfaces
{
    public interface IUserClaim
    {
        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        string ClaimValue { get; set; }
    }
}