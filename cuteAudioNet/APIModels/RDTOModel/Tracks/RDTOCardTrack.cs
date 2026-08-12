namespace cuteAudioNet.APIModels.RDTOModel.Tracks
{
    public record RDTOCardTrack(string Name = "",MusicGenre Genre = MusicGenre.None,string Artist = "");


    /// <summary>
    ///    None = 0,
    ///    POP = 1,
    ///    ROCK = 2,
    ///    RAP = 3,
    ///    ELECTRONIC = 4,
    ///    JAZZ = 5,
    ///    CLASSICAL = 6
    /// </summary>
    public enum MusicGenre
    {
        None = 0,
        POP = 1,
        ROCK = 2,
        RAP = 3,
        ELECTRONIC = 4,
        JAZZ = 5,
        CLASSICAL = 6
    }
}
