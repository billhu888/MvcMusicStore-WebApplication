using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using MvcMusicStore.Models;
using MvcMusicStore.Repositories;

namespace MvcMusicStore.Controllers
{
    public class StoreManagerController : Controller
    {
        public IActionResult Index()
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo(); 
            AlbumList albumlist = new AlbumList();

            ViewData["isPost"] = false;

            albumlist.albumlist = new List<Album>();

            bool success = storemanagerrepo.AllAlbumsList(albumlist.albumlist);

            if (success)
            {
                return View(albumlist);
            }
            else
            {
                return View();
            }
        }

        public IActionResult AddNewAlbum()
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album NewAlbum = new Album();

            ViewData["isPost"] = false;     

            NewAlbum.SelectGenre = new List<SelectListItem>();
            bool success2 = storemanagerrepo.GetNewAlbumListGenre(NewAlbum.SelectGenre);

            NewAlbum.SelectArtist = new List<SelectListItem>();
            bool success3 = storemanagerrepo.GetNewAlbumListArtist(NewAlbum.SelectArtist);

            if (success2 && success3)
            {
                return View(NewAlbum);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult AddNewAlbum(Album NewAlbum)
        {
            // Does not work
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            NewAlbum.SelectGenre = new List<SelectListItem>();
            NewAlbum.SelectArtist = new List<SelectListItem>();

            ViewData["isPost"] = true;
            ViewData["indicator"] = false;

            if (NewAlbum.SelectGenreId != null)
            {
                List<SelectListItem> ChosenGenre = NewAlbum.SelectGenre.Where
                    (p => NewAlbum.SelectGenreId.Contains(int.Parse(p.Value))).ToList();

                foreach (var genre in ChosenGenre)
                {
                    genre.Selected = true;
                }
            }

            if (NewAlbum.SelectArtistId != null)
            {
                List<SelectListItem> ChosenArtist = NewAlbum.SelectArtist.Where
                    (p => NewAlbum.SelectArtistId.Contains(int.Parse(p.Value))).ToList();

                foreach (var artist in ChosenArtist)
                {
                    artist.Selected = true;
                }
            }

            bool success1 = storemanagerrepo.GetNewAlbumID(NewAlbum);
            bool success2 = storemanagerrepo.AddNewAlbum(NewAlbum);

            if (success1 && success2)
            {
                ViewData["indicator"] = true;

                return View(NewAlbum);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Edit(int AlbumId, int GenreId, int ArtistId) 
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album album = new Album();

            ViewData["isPost"] = true;

            bool success1 = storemanagerrepo.RetrieveAlbumInfo(album, AlbumId);

            album.SelectGenre = new List<SelectListItem>();
            bool success2 = storemanagerrepo.ListGenres(album.SelectGenre, GenreId);

            album.SelectArtist = new List<SelectListItem>();
            bool success3 = storemanagerrepo.ListArtists(album.SelectArtist, ArtistId);

            if (success1 && success2 && success3)
            {
                return View(album);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(Album EditedAlbum)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            EditedAlbum.SelectGenre = new List<SelectListItem>();
            EditedAlbum.SelectArtist = new List<SelectListItem>();

            ViewData["isPost"] = true;
            ViewData["indicator"] = false;

            //Does not work
            //bool success1 = storemanagerrepo.Edit(EditedAlbum, EditedAlbum.AlbumId); 

            if(EditedAlbum.SelectGenreId != null)
            {
                List<SelectListItem> ChosenGenre = EditedAlbum.SelectGenre.Where
                    (p => EditedAlbum.SelectGenreId.Contains(int.Parse(p.Value))).ToList();

                foreach(var genre in ChosenGenre) 
                {
                    genre.Selected = true;
                }
            }

            if (EditedAlbum.SelectArtistId != null)
            {
                List<SelectListItem> ChosenArtist = EditedAlbum.SelectArtist.Where
                    (p => EditedAlbum.SelectArtistId.Contains(int.Parse(p.Value))).ToList();

                foreach (var artist in ChosenArtist)
                {
                    artist.Selected = true;
                }
            }
  
            bool success2 = storemanagerrepo.UpdateAlbum(EditedAlbum);

            if (success2)
            {
                ViewData["indicator"] = true;

                return View(EditedAlbum);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Details (int AlbumId) 
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo(); 
            Album album = new Album();

            bool success = storemanagerrepo.RetrieveAlbumInfo(album, AlbumId);

            if (success)
            {
                return View(album);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Delete(int AlbumId)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album album = new Album();

            bool success = storemanagerrepo.RetrieveAlbumInfo(album, AlbumId);

            if (success)
            {
                return View(album);
            }
            else
            {
                return View();
            }
        }

        public IActionResult DeleteConfirmation(int AlbumId) 
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album album = new Album();

            bool success = storemanagerrepo.RetrieveAlbumInfo(album, AlbumId);

            if (success)
            {
                return View(album);
            }
            else
            {
                return View();
            }
        }

        public IActionResult DeleteAlbum(int AlbumId)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();

            storemanagerrepo.DeleteAlbum(AlbumId);

            return View();

        }
    }
}