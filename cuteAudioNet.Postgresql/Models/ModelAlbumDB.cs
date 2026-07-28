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
}
