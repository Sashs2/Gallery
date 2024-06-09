namespace Gallery.Models
{
    public class DisLike
    {
        public int DisLikeId { get; set; }
        public string? UserId { get; set; }
        public int PhotoId { get; set; }
        public bool IsDisLike { get; set; }
        public ApplicationUser? User { get; set; }
        public Photo? Photo { get; set; }
    }
}
