using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using MvcMusicStore.Models;
using MvcMusicStore.Repositories;

namespace MvcMusicStore.Controllers
{
    public class StoreManagerController : Controller
    {
        /// <summary>
        ///     GET: /StoreManager
        ///     Shows the list of albums in the inventory 
        ///     Can edit, see the details of, and delete albums in the inventory
        ///     Can add new albums to the inventory
        /// </summary>
        /// 
        /// <returns>
        ///     A list of all the albums in the inventory
        ///     Ability to edit, see the details of, and delete albums in the inventory
        ///     Add new albums to the inventory
        /// </returns>

        public IActionResult Index()
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo(); 
            AlbumList albumlist = new AlbumList();

            ViewData["isPost"] = false;

            // Create a list to hold all the albums in the inventory
            albumlist.albumlist = new List<Album>();

            // Get the list of all the albums in the inventory
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

        /// <summary>
        ///     GET: /StoreManager/AddNewAlbum
        ///     Add a new album to the inventory by providing all the details of the new album
        /// </summary>
        /// 
        /// <returns>
        ///     A form to add a new album to the inventory and provide all of the album's details
        /// </returns>

        public IActionResult AddNewAlbum()
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album NewAlbum = new Album();

            ViewData["isPost"] = false;     

            // Provide a list to hold all of the album genres
            NewAlbum.SelectGenre = new List<SelectListItem>();

            // Get the list of all of the album genres
            bool success2 = storemanagerrepo.GetNewAlbumListGenre(NewAlbum.SelectGenre);


            // Provide a list of all of the artists
            NewAlbum.SelectArtist = new List<SelectListItem>();

            // Get the list of all of the artists
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

        /// <summary>
        ///     POST: /StoreManager/AddNewAlbum
        ///     
        /// </summary>
        /// 
        /// <param name="NewAlbum">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

        [HttpPost]
        public IActionResult AddNewAlbum(Album NewAlbum)
        {
            // Does not work
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

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

        /// <summary>
        ///     GET: /StoreManager/Edit
        /// </summary>
        /// 
        /// <param name="AlbumId">
        /// 
        /// </param>
        /// 
        /// <param name="GenreId">
        /// 
        /// </param>
        /// 
        /// <param name="ArtistId">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

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

        /// <summary>
        ///     POST: /StoreManager/Edit
        /// </summary>
        /// 
        /// <param name="EditedAlbum">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

        [HttpPost]
        public IActionResult Edit(Album EditedAlbum)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            EditedAlbum.SelectGenre = new List<SelectListItem>();
            EditedAlbum.SelectArtist = new List<SelectListItem>();

            ViewData["isPost"] = true;
            ViewData["indicator"] = false;

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

        /// <summary>
        ///     GET: /StoreManager/Details
        /// </summary>
        /// 
        /// <param name="AlbumId">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

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

        /// <summary>
        ///     GET: /StoreManager/Delete
        /// </summary>
        /// 
        /// <param name="AlbumId">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

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

        /// <summary>
        ///     GET: /StoreManager/DeleteConfirmation
        /// </summary>  
        /// 
        /// <param name="AlbumId">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

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

        /// <summary>
        ///     GET: /StoreManager/DeleteAlbum
        /// </summary>
        /// 
        /// <param name="AlbumId">
        /// 
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

        public IActionResult DeleteAlbum(int AlbumId)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();

            storemanagerrepo.DeleteAlbum(AlbumId);

            return View();

        }
    }
}