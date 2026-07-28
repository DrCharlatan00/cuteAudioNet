namespace cuteAudioNet.APIModels.RDTOModel
{
    public record RDTOCardTrack(string Name = "",MusicGenre Genre = MusicGenre.None,string Artist = "");


    public enum MusicGenre
    {
        None,
        POP,
        ROCK,
        RAP,
        ELECTRONIC,
        JAZZ,
        CLASSICAL
    }
}
