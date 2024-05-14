using Microsoft.AspNetCore.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using MvcMusicStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Repositories;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        /// <summary>
        ///     GET: /Store
        ///     List all the genres and if you click on the genre
        ///         it will show all the albums of that genre
        /// </summary>
        /// 
        /// <returns>
        ///     List all the genres and if you click on the genre
        ///         it will show all the albums of that genre
        /// </returns>

        public ActionResult Index()
        {
            StoreRepo storeRepo = new StoreRepo();
            GenreList GenresList = new GenreList();

            // Create a list to hold all the genres
            GenresList.GenresList = new List<Genre>();

            // Get the list of all the genres
            bool success = storeRepo.GenresList(GenresList.GenresList);

            if (success)
            {
                return View(GenresList);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        ///     GET: /Store/Browse?genre=Jazz&genreId=1002
        /// </summary>
        /// 
        /// <param name="genre">
        ///     genre parameter to know the name of the genre selected
        /// </param>
        /// 
        /// <param name="genreID">
        ///     genreId parameter is to know the ID of the genre selected
        /// </param>
        /// 
        /// <returns>
        ///     A list of all the albums belonging to the selected genre
        /// </returns>
      
        public ActionResult Browse(String genre, int genreID)
        {
            StoreRepo storeRepo = new StoreRepo();
            Genre Genres = new Genre();

            Genres.Name = genre;

            // Get the list of albums belonging to the selected genre
            bool success = storeRepo.GenreAlbumsList(Genres, genreID);

            if (success)
            {
                return View(Genres);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        ///     GET: /Store/Details/5
        ///     Collects all the information of the album, including
        ///         the ability to add the album to your cart
        /// </summary>
        /// 
        /// <param name="AlbumId">
        ///     AlbumId provides the ID of the album so you can see 
        ///         the album's information and add it to your cart
        /// </param>
        /// 
        /// <returns>
        ///     Display all the information of the album, including 
        ///         the ability to add the album to your cart
        /// </returns>

        public ActionResult Details(int AlbumId)
        {
            StoreRepo storeRepo = new StoreRepo();
            Album album = new Album();

            // Collect all the information about the album
            bool success = storeRepo.AlbumDetails(album, AlbumId);

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
        ///     AJAX: /Store/IndexAJAX
        ///     Using AJAX to fetch the list of genres to display on the left side bar
        ///         of every page so someone can click on the genre and see the albums 
        ///         belonging to that genre on any page they are on
        /// </summary>
        /// 
        /// <returns>
        ///     A JSON string showing a list of all the genres
        /// </returns>
        
        public IActionResult IndexAJAX()
        {
            StoreRepo storeRepo = new StoreRepo();
            GenreList GenresList = new GenreList();

            // Create a list to hold all the genres
            GenresList.GenresList = new List<Genre>();

            // Retrieves and sotres the genres in the list
            bool success = storeRepo.GenresList(GenresList.GenresList);

            if (success)
            {
                return Json( GenresList.GenresList  );
            }
            else
            {
                return View();
            }
        }
    }
}