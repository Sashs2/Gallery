namespace Gallery.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string ?Url { get; set; }
        public string ?ThumbnailUrl { get; set; }
        public int AlbumId { get; set; }
        public Album ?Album { get; set; }
        public ICollection<Like> ?Likes { get; set; }
        public ICollection<DisLike> ?DisLikes { get; set; }
    }
}
