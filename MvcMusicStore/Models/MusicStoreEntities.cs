using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MvcMusicStore.Models
{
    public class MusicStoreEntities : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(DESKTOP-BM0O4AL)\\mssqllocaldb;Database=Music_Store;Trusted_Connection=True;");
        }
    }
}