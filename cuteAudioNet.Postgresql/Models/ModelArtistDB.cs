using System;
using System.Collections.Generic;
using System.Text;

namespace cuteAudioNet.Postgresql.Models
{
    public class ModelArtistDB
    {
        public Guid ID { get; set; }
        public required string ArtistName { get; set; }
        public required string NickName { get; set; }
        public string? Surname { get; set; }
        public DateTime? BornDate { get; set; }
        public string? Pathonymic { get; set; }
        public ICollection<ModelAlbumDB> Albums { get; set; }

        
    }
}
