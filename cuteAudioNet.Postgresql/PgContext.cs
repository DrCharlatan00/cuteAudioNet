using cuteAudioNet.Postgresql.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace cuteAudioNet.Postgresql
{
    public class PgContext : DbContext
    {
        public DbSet<ModelArtistDB> artists { get; set; }
        public DbSet<ModelAlbumDB> albums { get; set; }
        public DbSet<ModelTrackDB> tracks { get; set; }

        public PgContext(DbContextOptions<PgContext> options) : base (options) { }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelTrackDB>().HasOne(x => x.Album).WithMany(x => x.Tracks).HasForeignKey(x => x.AlbumID);
            modelBuilder.Entity<ModelTrackDB>().HasIndex(x => x.Name).HasMethod("gin").HasOperators("gin_trgm_ops");
            
            modelBuilder.Entity<ModelAlbumDB>().HasOne(x => x.Artist).WithMany(x => x.Albums).HasForeignKey(x => x.ArtistID);
            modelBuilder.Entity<ModelAlbumDB>().HasIndex(x => x.AlbumName).HasMethod("gin").HasOperators("gin_trgm_ops");

            
            modelBuilder.Entity<ModelArtistDB>().HasIndex(x => x.NickName).HasMethod("gin").HasOperators("gin_trgm_ops");
            base.OnModelCreating(modelBuilder);
        }
    }
}
