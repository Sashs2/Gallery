namespace Gallery.Models
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string ?Title { get; set; }
        public string ?Description { get; set; }
        public string ?CoverUrl { get; set; }
        public string ?UserId { get; set; }
        public ApplicationUser ?User { get; set; }
        public ICollection<Photo> ?Photos { get; set; }
    }
}
