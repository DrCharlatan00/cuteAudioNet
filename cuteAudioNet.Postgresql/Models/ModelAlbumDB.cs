using System;
using System.Collections.Generic;
using System.Text;

namespace cuteAudioNet.Postgresql.Models
{
    public class ModelAlbumDB
    {
        public required Guid ID { get; set; }
        public string AlbumName { get; set; }
        public DateTime? DateRelease { get; set; }
        public Guid ArtistID { get; set; }

        public ModelArtistDB Artist { get; set; }

        public ICollection<ModelTrackDB> Tracks { get; set; }
    }

    public class ModelAlbumCardDb {
        private string albumName;
        private string nickName;

        public ModelAlbumCardDb(string albumName, string nickName)
        {
            this.albumName = albumName;
            this.nickName = nickName;
        }

        public string Name { get; set; } = "Unknown";
        public string ArtistName { get; set; } = "Unknown"; 
    }
}
