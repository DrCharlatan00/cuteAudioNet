using cuteAudioNet.APIModels.RDTOModel.Tracks;

namespace cuteAudioNet.APIModels.DTO.Tracks
{
    public record DTOTrack(string Name, Guid AlbumID,MusicGenre Genre, string? TimeRelease, string? SubArtist);
}
