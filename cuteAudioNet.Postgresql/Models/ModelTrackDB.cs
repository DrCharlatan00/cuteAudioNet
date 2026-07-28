namespace cuteAudioNet.Postgresql.Models
{
    public class ModelTrackDB
    {
        public required Guid ID { get; set; }
        public Guid AlbumID { get; set; }
        public string Name { get; set; }
        public MusicGenre Genre { get; set; }
        public DateTime? TimeRelease { get; set; }
        public string? SubArtist { get; set; }
        public ModelAlbumDB Album { get; set; }
    }

    public enum MusicGenre {
        None,
        POP,
        ROCK,
        RAP,
        ELECTRONIC,
        JAZZ,
        CLASSICAL
    }
}
