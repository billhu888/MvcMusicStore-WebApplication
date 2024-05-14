using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models
{
    public class Album
    {
        public const string fAlbumId = "AlbumId";
        public const string fGenreId = "GenreId";
        public const string fGenreName = "GenreName";
        public const string fArtistId = "ArtistId";
        public const string fArtistName = "ArtistName";
        public const string fTitle = "Title";
        public const string fPrice = "Price";
        public const string fAlbumArtUrl = "AlbumArtUrl";
        public const string fGenre = "Genre";
        public const string fArtist = "Artist";

        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public string GenreName {  get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }

        [Display(Name = "Title1")]
        [Required(ErrorMessage = "An Album Title is required")]
        [StringLength(300)]
        public string Title { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        public decimal Price { get; set; }

        [Display(Name = "AlbumArtUrl")]
        [StringLength(300)]
        public string AlbumArtUrl { get; set; }

        public Genre Genre { get; set; }
        public List<SelectListItem> SelectGenre { get; set; }

        [Display(Name = "Genre")]
        [Required(ErrorMessage = "An Album Genre is Required")]
        public int[] SelectGenreId { get; set; }

        public Artist Artist { get; set; }
        public List<SelectListItem> SelectArtist { get; set; }

        [Display(Name = "Artist")]
        [Required(ErrorMessage = "An Album Artist is Required")]
        public int[] SelectArtistId { get; set; }
    }
}