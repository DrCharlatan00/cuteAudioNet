using cuteAudioNet.APIModels.RDTOModel;

namespace cuteAudioNet.APIModels.DTO
{
    public record DTOTrack(string Name, Guid AlbumID,MusicGenre Genre, string? TimeRelease, string? SubArtist);
}
