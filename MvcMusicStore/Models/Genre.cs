namespace MvcMusicStore.Models
{
    public class Genre
    {
        public const string fGenreId = "GenreId";
        public const string fName = "Name";
        public const string fDescription = "Description";

        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }
    }
}