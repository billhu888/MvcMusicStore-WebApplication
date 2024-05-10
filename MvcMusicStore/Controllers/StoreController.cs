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

        public ActionResult Index()
        {
            StoreRepo storeRepo = new StoreRepo();
            GenreList GenresList = new GenreList();

            GenresList.GenresList = new List<Genre>();

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

        // GET: /Store/Browse?genre=Disco
        public ActionResult Browse(String genre, int genreID)
        {
            StoreRepo storeRepo = new StoreRepo();
            Genre Genres = new Genre();

            Genres.Name = genre;

            bool success = storeRepo.GenreAlbumsList(Genres, genre, genreID);

            if (success)
            {
                return View(Genres);
            }
            else
            {
                return View();
            }
        }

        //GET: /Store/Details/5
        public ActionResult Details(int AlbumId)
        {
            StoreRepo storeRepo = new StoreRepo();
            Album album = new Album();

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

        public IActionResult IndexAJAX()
        {
            StoreRepo storeRepo = new StoreRepo();
            GenreList GenresList = new GenreList();

            GenresList.GenresList = new List<Genre>();

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