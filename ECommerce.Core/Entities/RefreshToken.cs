namespace ECommerce.Core.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn is null && !IsExpired;

        //Parent -> Child(RefreshToken)

        //One to Many Relation With ApplicationUser ( ApplicationUser (1) -> (N) RefreshToken )
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

    }
}
