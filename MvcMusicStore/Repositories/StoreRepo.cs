using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcMusicStore.Models;
using System.Data;
using MvcMusicStore.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MvcMusicStore.Repositories;
using System.Diagnostics.Eventing.Reader;

namespace MvcMusicStore.Repositories
{
    class StoreRepo : BaseRepo
    {
        /// <summary>
        ///     Gets the list of all the genres
        /// </summary>
        /// 
        /// <param name="GenresList">
        ///     A list that will hold all the genres
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that all the genres 
        ///        have been added to the list of genres
        /// </returns>

        public bool GenresList(List<Genre> GenresList)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT * " +
                        $" FROM Genre ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0 ) 
                    { 
                        Genre genre;

                        // Goes throw each row (genre) to add to list of genres
                        foreach(DataRow row in dt.Rows) 
                        {
                            // Fills in the model of each genre
                            genre = new Genre() 
                            {
                                GenreId = (int)row[Genre.fGenreId],
                                Name = (string)row[Genre.fName],
                            };

                            // Adds each genre to the list of genres
                            GenresList.Add(genre);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No genres found");
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Gets the list of albums belonging to the selected genre
        /// </summary>
        /// 
        /// <param name="Genre">
        ///     Model that will hold all the albums belonging to the 
        ///        selected genre
        /// </param>
        /// 
        /// <param name="genreid">
        ///     Id of the selected genre
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that it has successfully retrieved and added
        ///        all the albums of the selected genre to the list
        /// </returns>

        public bool GenreAlbumsList(Genre Genre, int genreid)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT * " +
                        $" FROM Album " +
                        $" WHERE genreid = {genreid}";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Genre.Albums = new List<Album>();
                        Album album;

                        // Goes throw each row (album)
                        foreach (DataRow row in dt.Rows)
                        {
                            // Fills in the model of each album
                            album = new Album()
                            {
                                AlbumId = (int)row[Album.fAlbumId],
                                GenreId = (int)row[Album.fGenreId],
                                ArtistId = (int)row[Album.fArtistId],
                                Title = (string)row[Album.fTitle],
                                Price = (decimal)row[Album.fPrice],
                                AlbumArtUrl = (string)row[Album.fAlbumArtUrl]
                            };

                            // Adds each album to the list of albums belonging
                            //    to the specific genre 
                            Genre.Albums.Add(album);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No albums found");
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Provides the details of each album
        /// </summary>
        /// 
        /// <param name="album">
        ///     Model that will hold the information of the selected album
        /// </param>
        /// 
        /// <param name="albumId">
        ///     Id of the selected album
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the details of the selected album
        ///        have been successful retrieve and added to the model
        /// </returns>

        public bool AlbumDetails(Album album, int albumId)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT * " +
                        $" FROM Album " +
                        $" WHERE AlbumId = {albumId} ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    // Checks if the selected album Id is in the database
                    if (dt.Rows.Count == 1)
                    {
                        // Adds the album's information to the model
                        foreach (DataRow row in dt.Rows)
                        {
                            album.AlbumId = (int)row[Album.fAlbumId];
                            album.GenreId = (int)row[Album.fGenreId];
                            album.ArtistId = (int)row[Album.fArtistId];
                            album.Title = (string)row[Album.fTitle];
                            album.Price = (decimal)row[Album.fPrice];
                            album.AlbumArtUrl = (string)row[Album.fAlbumArtUrl];
                        }
                    }
                    else
                    {
                        Console.WriteLine("Album ID does not belong to 1 album");
                    }

                    // Checks if the album's genre id is in the database
                    String sql2 =
                        $" SELECT * " +
                        $" FROM Genre " +
                        $" Where GenreId = {album.GenreId}";

                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter();
                    da2.SelectCommand = new SqlCommand(sql2, connection);
                    da2.Fill(dt2);

                    // Checks if the selected genre name is in the database
                    if (dt2.Rows.Count == 1)
                    {
                        // Adds the album's genre name to the model
                        foreach (DataRow row in dt2.Rows)
                        {
                            album.GenreName = (string)row["Name"];
                        }
                    }
                    else
                    {
                        Console.WriteLine("Genre ID does not belong to 1 genre");
                    }

                    String sql3 =
                        $" SELECT * " +
                        $" FROM Artist " +
                        $" Where ArtistId = {album.ArtistId}";

                    DataTable dt3 = new DataTable();
                    SqlDataAdapter da3 = new SqlDataAdapter();
                    da3.SelectCommand = new SqlCommand(sql3, connection);
                    da3.Fill(dt3);

                    // Checks if the album's artist is in the database
                    if (dt3.Rows.Count == 1)
                    {
                        // Adds the album's artist to the model
                        foreach (DataRow row in dt3.Rows)
                        {
                            album.ArtistName = (string)row["Name"];
                        }
                    }
                    else
                    {
                        Console.WriteLine("Artist ID does not belong to 1 artist");
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }
    }
}