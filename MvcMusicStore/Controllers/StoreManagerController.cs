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

            // Provide a dropdown list to hold all of the album genres
            NewAlbum.SelectGenre = new List<SelectListItem>();

            // Get the list of all of the album genres
            bool success2 = storemanagerrepo.GetNewAlbumListGenre(NewAlbum.SelectGenre);


            // Provide a dropdown list of all of the artists
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
        ///     After a new album's information is submitted, it adds the new album
        ///         and it's information to the database
        /// </summary>
        /// 
        /// <param name="NewAlbum">
        ///     Holds the new album's information
        /// </param>
        /// 
        /// <returns>
        ///     A page that says the new album has been successfully added to the database
        /// </returns>

        [HttpPost]
        public IActionResult AddNewAlbum(Album NewAlbum)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();

            // Set up an object for a dropdown list of all the genre selections
            NewAlbum.SelectGenre = new List<SelectListItem>();

            // Set up an object for a dropdown list for all the artist selections
            NewAlbum.SelectArtist = new List<SelectListItem>();

            ViewData["isPost"] = true;
            ViewData["indicator"] = false;

            // Checks if the list of selected music genre IDs is not null
            if (NewAlbum.SelectGenreId != null)
            {
                // Checks which genres (by ID) in SelectGenre (contains all possible genres) 
                //    are also in SelectGenreId (containing the genre(s) selected)
                List<SelectListItem> ChosenGenre = NewAlbum.SelectGenre.Where
                    (p => NewAlbum.SelectGenreId.Contains(int.Parse(p.Value))).ToList();

                foreach (var genre in ChosenGenre)
                {
                    genre.Selected = true;
                }
            }
            
            if (NewAlbum.SelectArtistId != null)
            {
                // Checks which artists (by ID) in SelectArtist (contains all possible artists) 
                //    are also in SelectArtistId (containing the artist(s) selected)
                List<SelectListItem> ChosenArtist = NewAlbum.SelectArtist.Where
                    (p => NewAlbum.SelectArtistId.Contains(int.Parse(p.Value))).ToList();

                foreach (var artist in ChosenArtist)
                {
                    artist.Selected = true;
                }
            }

            bool success1 = storemanagerrepo.GetNewAlbumID(NewAlbum);
            bool success2 = storemanagerrepo.CheckNewAlbumGenreId(NewAlbum);
            bool success3 = storemanagerrepo.CheckNewAlbumArtistId(NewAlbum);
            bool success4 = storemanagerrepo.CheckNewAlbumTitle(NewAlbum.Title);
            bool success5 = storemanagerrepo.CheckNewAlbumPrice(NewAlbum.Price);
            bool success6 = storemanagerrepo.CheckNewAlbumArtURL(NewAlbum.AlbumArtUrl);

            if (success1 && success2 && success3 & success4 && success5 && success6)
            {
                bool success7 = storemanagerrepo.AddNewAlbum(NewAlbum);

                ViewData["indicator"] = true;

                if (success7 == true)
                {
                    return View(NewAlbum);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                NewAlbum.SelectGenre = new List<SelectListItem>();
                storemanagerrepo.GetNewAlbumListGenre(NewAlbum.SelectGenre);


                NewAlbum.SelectArtist = new List<SelectListItem>();
                storemanagerrepo.GetNewAlbumListArtist(NewAlbum.SelectArtist);

                return View(NewAlbum);
            }
        }

        /// <summary>
        ///     GET: /StoreManager/Edit
        ///     Provides you a form on how to edit an album's information
        /// </summary>
        /// 
        /// <param name="AlbumId">
        ///     Tells which album's information to edit
        /// </param>
        /// 
        /// <param name="GenreId">
        ///     Tells the id of the genre of the album to be edited
        /// </param>
        /// 
        /// <param name="ArtistId">
        ///     Tells the id of the artist of the album to be edited
        /// </param>
        /// 
        /// <returns>
        ///     Provides a form to edit an album's information
        /// </returns>

        public IActionResult Edit(int AlbumId, int GenreId, int ArtistId) 
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();
            Album album = new Album();

            ViewData["isPost"] = false;

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
        ///     After an album's information is edited, it updates the album's information
        ///         in the database
        /// </summary>
        /// 
        /// <param name="EditedAlbum">
        ///     Provides the updated information of the album
        /// </param>
        /// 
        /// <returns>
        ///     Tells you that the album's information was successfully updated
        /// </returns>

        [HttpPost]
        public IActionResult Edit(Album EditedAlbum)
        {
            StoreManagerRepo storemanagerrepo = new StoreManagerRepo();

            // Set up an object for a dropdown list of all the genre selections
            EditedAlbum.SelectGenre = new List<SelectListItem>();

            // Set up an object for a dropdown list for all the artist selections
            EditedAlbum.SelectArtist = new List<SelectListItem>();

            ViewData["isPost"] = true;
            ViewData["indicator"] = false;

            // Checks if the list of selected music genre IDs is not null
            if (EditedAlbum.SelectGenreId != null)
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

            bool success1 = storemanagerrepo.GetNewAlbumID(EditedAlbum);
            bool success2 = storemanagerrepo.CheckNewAlbumGenreId(EditedAlbum);
            bool success3 = storemanagerrepo.CheckNewAlbumArtistId(EditedAlbum);
            bool success4 = storemanagerrepo.CheckNewAlbumTitle(EditedAlbum.Title);
            bool success5 = storemanagerrepo.CheckNewAlbumPrice(EditedAlbum.Price);
            bool success6 = storemanagerrepo.CheckNewAlbumArtURL(EditedAlbum.AlbumArtUrl);

            if (success1 && success2 && success3 & success4 && success5 && success6)
            {
                bool success7 = storemanagerrepo.AddNewAlbum(EditedAlbum);

                ViewData["indicator"] = true;

                if (success7 == true)
                {
                    return View(EditedAlbum);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                EditedAlbum.SelectGenre = new List<SelectListItem>();
                storemanagerrepo.GetNewAlbumListGenre(EditedAlbum.SelectGenre);


                EditedAlbum.SelectArtist = new List<SelectListItem>();
                storemanagerrepo.GetNewAlbumListArtist(EditedAlbum.SelectArtist);

                return View(EditedAlbum);
            }
        }

        /// <summary>
        ///     GET: /StoreManager/Details
        ///     Provides the details of an album when the album is clicked in StoreManager
        /// </summary>
        /// 
        /// <param name="AlbumId">
        ///     Provides the ID of the album that was clicked on 
        /// </param>
        /// 
        /// <returns>
        ///     The clicked on Album's details
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