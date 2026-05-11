namespace Myshop.Core.Models
{
    public class UserFollow
    {
        public int Id { get; set; }
        public string FollowerId { get; set; } = string.Empty;
        public string FollowingId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? Follower { get; set; }
        public AppUser? Following { get; set; }
    }
}
