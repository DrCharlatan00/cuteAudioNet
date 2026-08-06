namespace cuteAudioNet.APIModels.RDTOModel.Albums
{
    public record RDTOAlbumCard(string Name, string ArtistName);
    public record RDTOAlbum(string Name,string? DateRelease,string ArtistName,IEnumerable<Tracks.RDTOTrack> Tracks);
}
