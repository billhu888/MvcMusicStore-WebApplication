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

                        foreach (DataRow row in dt.Rows)
                        {
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
                        foreach (DataRow row in dt.Rows)
                        {
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
                        foreach (DataRow row in dt.Rows)
                        {
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
                        $" '{NewAlbum.Title}', {NewAlbum.Price}, '{NewAlbum.AlbumArtUrl}') ";

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
                        foreach (DataRow row in dt.Rows)
                        {
                            String SelectedGenreId = GenreId.ToString();

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
                        foreach (DataRow row in dt.Rows)
                        {
                            String SelectedArtistId = ArtistId.ToString();

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

                    foreach (int id in Album.SelectGenreId)
                    {
                        GenreId = id;
                    }

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

        public bool ConfirmDeleteAlbum()
        {
            bool success = false;

            return success;
        }

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