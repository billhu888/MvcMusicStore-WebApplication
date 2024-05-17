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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcMusicStore.Repositories
{
    class StoreManagerRepo : BaseRepo
    {
        /// <summary>
        ///     Add all the albums to a list
        /// </summary>
        /// 
        /// <param name="listalbums">
        ///     Provides a list where all the albums will be stored in
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that all the albums have been added to the list of albums
        /// </returns>
        
        public bool AllAlbumsList(List<Album> listalbums)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT Album.AlbumId, Album.GenreId, Genre.Name AS GenreName, " +
                        $" Album.ArtistId, Artist.Name as ArtistName, Album.Title, " +
                        $" Album.Price, Album.AlbumArtUrl " +
                        $" FROM Album " +
                        $" INNER JOIN Artist on Album.ArtistId = Artist.ArtistId " +
                        $" INNER JOIN Genre on Album.GenreId = Genre.GenreId ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Album album;

                        // Goes through every row (album) in the table 
                        foreach (DataRow row in dt.Rows)
                        {
                            // Adds every detail of the album to the album model
                            album = new Album()
                            {
                                AlbumId = (int)row[Album.fAlbumId],
                                GenreId = (int)row[Album.fGenreId],
                                GenreName = (string)row[Album.fGenreName],
                                ArtistId = (int)row[Album.fArtistId],
                                ArtistName = (string)row[Album.fArtistName],
                                Title = (string)row[Album.fTitle],
                                Price = (decimal)row[Album.fPrice],
                                AlbumArtUrl = (string)row[Album.fAlbumArtUrl]
                            };

                            // Adds the album to the list of albums
                            listalbums.Add(album);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No albums found");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }

                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Add all the album genres to a list
        /// </summary>
        /// 
        /// <param name="ListGenres">
        ///     Provides a list where all the genres will be stored in
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that all the genres have been added to the list of genres
        /// </returns>

        public bool GetNewAlbumListGenre(List<SelectListItem> ListGenres)
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

                    if (dt.Rows.Count > 0)
                    {
                        // Goes through every row (genre) in the table
                        foreach (DataRow row in dt.Rows)
                        {
                            // Adds the list of genres to a list
                            ListGenres.Add(new SelectListItem
                            {
                                Text = row["Name"].ToString(),
                                Value = row["GenreId"].ToString()
                            });
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
        ///     Adds all the artists to a list
        /// </summary>
        /// 
        /// <param name="ListArtists">
        ///     A list to hold all the artists
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that all the artists have succesfully been added
        ///        to the list of artists
        /// </returns>

        public bool GetNewAlbumListArtist(List<SelectListItem> ListArtists)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT * " +
                        $" FROM Artist ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Goes through every row (artist) in the table
                        foreach (DataRow row in dt.Rows)
                        {
                            // Adds the list of artists to a list
                            ListArtists.Add(new SelectListItem
                            {
                                Text = row["Name"].ToString(),
                                Value = row["ArtistId"].ToString()
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("No artists found");
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
        ///     Generates the ID for a newly added album
        /// </summary>
        /// 
        /// <param name="Album">
        ///     Model that will hold the ID of the newly added album
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the ID for the newly added album has been 
        ///        successfully generated and added to the model
        /// </returns>

        public bool GetNewAlbumID(Album Album)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT COUNT(*) " +
                        $" FROM ALBUM ";

                    SqlCommand cmd = new SqlCommand(sql, connection);
                    int rowCount = (int)cmd.ExecuteScalar();

                    // When generating the Album ID it checks if there are already 
                    //    albums in the database to determine what the ID will be
                    if (rowCount == 0)
                    {
                        Album.AlbumId = 100001;
                    }
                    else
                    {
                        String sql2 =
                            $" SELECT MAX(AlbumId) AS MaxAlbumId" +
                            $" FROM Album ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql2, connection);
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row1 = dt.Rows[0];

                            Album.AlbumId = (int)row1["MaxAlbumId"] + 1;
                        }
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
        ///     Checks if the selected genre of the new album is valid
        /// </summary>
        /// 
        /// <param name="NewAlbum">
        ///     Model that holds the ID of the selected genre
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the selected genre is valid
        /// </returns>

        public bool CheckNewAlbumGenreId(Album NewAlbum)
        {
            bool success = false;

            // Goes through the ID of the selected genre to see if it is valid
            foreach (int id in NewAlbum.SelectGenreId)
            {
                success = true;
            }     

            return success;
        }

        /// <summary>
        ///     Checks if the selected artist of the new album is valid
        /// </summary>
        /// 
        /// <param name="NewAlbum">
        ///     Model that holds the ID of the selected artist
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the selected artist is valid
        /// </returns>

        public bool CheckNewAlbumArtistId(Album NewAlbum)
        {
            bool success = false;

            // Goes through the ID of the selected artist to see if it is valid
            foreach (int id in NewAlbum.SelectArtistId)
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Checks if the title of the new album is not empty or not null
        /// </summary>
        /// 
        /// <param name="Title">
        ///     Holds the title of the new album
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the title of the new album is 
        ///        not empty or not null
        /// </returns>

        public bool CheckNewAlbumTitle(String Title)
        {
            bool success = false;

            // Checks if the title of the new album is not empty or not null
            if (!string.IsNullOrWhiteSpace(Title))
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Checks if the price of the new album is bigger than 0
        /// </summary>
        /// 
        /// <param name="Price">
        ///     Holds the price of the new album
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the price of the new album is bigger than 0
        /// </returns>

        public bool CheckNewAlbumPrice(decimal Price)
        {
            bool success = false;

            // Checks if the price if the new album is bigger than 0
            if (Price > 0)
            {
                success = true;
            }

            return success;
        }


        /// <summary>
        ///     Checks if the albumarturl of the new album is not empty or not null
        /// </summary>
        /// 
        /// <param name="AlbumArtUrl">
        ///     Holds the albumarturl
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the albumarturl of the new album is not empty
        ///        or not null
        /// </returns>

        public bool CheckNewAlbumArtURL(String AlbumArtUrl)
        {
            bool success = false;

            // Checks if the albumarturl of the new album is not empty or not null
            if (!string.IsNullOrWhiteSpace(AlbumArtUrl))
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Adds the new album to the database
        /// </summary>
        /// 
        /// <param name="NewAlbum">
        ///     Model that holds the information of the model
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the new album and its information
        ///        have been successfully added to the album database
        /// </returns>

        public bool AddNewAlbum(Album NewAlbum)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    int GenreId = 0;
                    int ArtistId = 0;

                    foreach (int id in NewAlbum.SelectGenreId)
                    {
                        GenreId = id;
                    }

                    foreach (int id in NewAlbum.SelectArtistId)
                    {
                        ArtistId = id;
                    }

                    String sql =
                        $" INSERT INTO Album " +
                        $" (GenreId, ArtistId, Title, Price, AlbumArtUrl) " +
                        $" Values ({GenreId}, {ArtistId}, " +
                        $"    '{NewAlbum.Title}', {NewAlbum.Price}, '{NewAlbum.AlbumArtUrl}') ";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

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
        ///     Retrieves the information of the requested album
        /// </summary>
        /// 
        /// <param name="album">
        ///     Model that will hold the information of the requested album
        /// </param>
        /// 
        /// <param name="albumId">
        ///     Id of the album whose information you want
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the requested album's information has been successfully
        ///        retrieved and bound to the album model
        /// </returns>

        public bool RetrieveAlbumInfo(Album album, int albumId)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT Album.AlbumId, Album.GenreId, Genre.Name AS GenreName, " +
                        $" Album.ArtistId, Artist.Name as ArtistName, Album.Title, " +
                        $" Album.Price, Album.AlbumArtUrl " +
                        $" FROM Album " +
                        $" INNER JOIN Artist on Album.ArtistId = Artist.ArtistId " +
                        $" INNER JOIN Genre on Album.GenreId = Genre.GenreId " +
                        $" WHERE AlbumId = {albumId}";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);
                    var countrow = dt.Rows.Count;

                    if (dt.Rows.Count == 1)
                    {
                        // Binds the requested album's information to the model
                        foreach (DataRow row in dt.Rows)
                        {
                            album.AlbumId = (int)row[Album.fAlbumId];
                            album.GenreId = (int)row[Album.fGenreId];
                            album.GenreName = (string)row[Album.fGenreName];
                            album.ArtistId = (int)row[Album.fArtistId];
                            album.ArtistName = (string)row[Album.fArtistName];
                            album.Title = (string)row[Album.fTitle];
                            album.Price = (decimal)row[Album.fPrice];
                            album.AlbumArtUrl = (string)row[Album.fAlbumArtUrl];
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID does not belong to 1 album");
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
        ///     Retrieves the list of all the genres
        /// </summary>
        /// 
        /// <param name="SelectGenre">
        ///     A list that will hold all the genres
        /// </param>
        /// 
        /// <param name="GenreId">
        ///     Holds the Id of the genre that was previously selected
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that it has successfully collected and added the list of genres  
        ///        and to display by default the last genre selected for the album
        /// </returns>

        public bool ListGenres(List<SelectListItem> SelectGenre, int? GenreId)
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

                    if (dt.Rows.Count > 0)
                    {
                        // Goes through the rows (genres) 
                        foreach (DataRow row in dt.Rows)
                        {
                            String SelectedGenreId = GenreId.ToString();

                            // Adds each genre to the list of genres
                            SelectGenre.Add(new SelectListItem
                            {
                                Text = row["Name"].ToString(),
                                Value = row["GenreId"].ToString(),
                                Selected = (row["GenreId"].ToString() == SelectedGenreId)
                            });
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
        ///     Retrieves a list of all the artists
        /// </summary>
        /// 
        /// <param name="SelectArtist">
        ///     List to hold ann the artists
        /// </param>
        /// 
        /// <param name="ArtistId">
        ///     Holds the Id of the artist that was previously selected
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that it has successfully collected and added the list of artists 
        ///        and to display by default the last artist selected for the album
        /// </returns>

        public bool ListArtists(List<SelectListItem> SelectArtist, int? ArtistId)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT * " +
                        $" FROM Artist ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Goes through the rows (artists)
                        foreach (DataRow row in dt.Rows)
                        {
                            String SelectedArtistId = ArtistId.ToString();

                            // Adds each artist to the list of artists
                            SelectArtist.Add(new SelectListItem
                            {
                                Text = row["Name"].ToString(),
                                Value = row["ArtistId"].ToString(),
                                Selected = (row["ArtistId"].ToString() == SelectedArtistId)
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("No artists found");
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
        ///     Updates the album with the updated information in the database
        /// </summary>
        /// 
        /// <param name="Album">
        ///     Model that holds the updated information for the album
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the album's information has been 
        ///        successfully updated in the database
        /// </returns>

        public bool UpdateAlbum(Album Album)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    int GenreId = 0;
                    int ArtistId = 0;
                    string Title = Album.Title;

                    // Gets the album's genre Id so it can be used in
                    //    updating the database
                    foreach (int id in Album.SelectGenreId)
                    {
                        GenreId = id;
                    }

                    // Gets the album's artist Id so it can be used in
                    //    updating the database
                    foreach (int id in Album.SelectArtistId)
                    {
                        ArtistId = id;
                    }

                    String sql =
                        $" UPDATE Album " +
                        $" SET GenreId = {GenreId}, ArtistId = {ArtistId}, " +
                        $" Title = '{Album.Title}', Price = {Album.Price}, " +
                        $" AlbumArtUrl = '{Album.AlbumArtUrl}' " +
                        $" WHERE AlbumId = {Album.AlbumId} ";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

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
        ///     Deletes the selected album from the database
        /// </summary>
        /// 
        /// 
        /// <param name="AlbumId">
        ///     Provides the Id of the album to be deleted
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the selected album has been 
        ///        deleted from the database
        /// </returns>

        public bool DeleteAlbum(int AlbumId) 
        { 
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" DELETE From Album " +
                        $" Where AlbumId = {AlbumId} ";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

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