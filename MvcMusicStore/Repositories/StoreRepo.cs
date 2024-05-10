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

                        foreach(DataRow row in dt.Rows) 
                        {
                            genre = new Genre() 
                            {
                                GenreId = (int)row[Genre.fGenreId],
                                Name = (string)row[Genre.fName],
                            };

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

        public bool GenreAlbumsList(Genre Genre, String genre, int genreid)
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

                        foreach (DataRow row in dt.Rows)
                        {
                            album = new Album()
                            {
                                AlbumId = (int)row[Album.fAlbumId],
                                GenreId = (int)row[Album.fGenreId],
                                ArtistId = (int)row[Album.fArtistId],
                                Title = (string)row[Album.fTitle],
                                Price = (decimal)row[Album.fPrice],
                                AlbumArtUrl = (string)row[Album.fAlbumArtUrl]
                            };

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

                    if (dt.Rows.Count == 1)
                    {
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

                    String sql2 =
                        $" SELECT * " +
                        $" FROM Genre " +
                        $" Where GenreId = {album.GenreId}";

                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter();
                    da2.SelectCommand = new SqlCommand(sql2, connection);
                    da2.Fill(dt2);

                    if (dt2.Rows.Count == 1)
                    {
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

                    if (dt3.Rows.Count == 1)
                    {
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