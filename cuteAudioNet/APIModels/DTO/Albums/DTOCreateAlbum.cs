namespace cuteAudioNet.APIModels.DTO.Albums
{
    public record DTOCreateAlbum(
        string Name, 
        string? DateRelease,
        Guid IdArtist
        );

    public record DTOUpdateAlbum(
    Guid id,
    string AlbumName,
    string? DateRelease
    );
}
