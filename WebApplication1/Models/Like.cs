namespace Gallery.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public string ?UserId { get; set; }
        public int PhotoId { get; set; }
        public bool IsLike { get; set; }
        public ApplicationUser ?User { get; set; }
        public Photo ?Photo { get; set; }
    }
}
