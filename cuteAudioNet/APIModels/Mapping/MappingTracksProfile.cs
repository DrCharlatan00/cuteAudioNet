using AutoMapper;
using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.APIModels.Mapping
{
    public class MappingTracksProfile : Profile
    {
        public MappingTracksProfile()
        {
            #region RDTOMap
            CreateMap<ModelTrackDB, RDTOCardTrack>()
                .ConstructUsing(src => new RDTOCardTrack(
                    src.Name,
                    Genre: (RDTOModel.Tracks.MusicGenre)src.Genre,
                    src.Album.Artist.NickName + "feat." + src.SubArtist
                    ));
            CreateMap<ModelTrackDB, RDTOTrack>()
                .ConstructUsing(src => new RDTOTrack(
                       Name: src.Name,

                      Artists: src.Album != null && src.Album.Artist != null
    ? src.Album.Artist.NickName + " feat. " + src.SubArtist
    : "N/A",

                       Genre: (RDTOModel.Tracks.MusicGenre)src.Genre,
                       TimeRelease: src.TimeRelease.HasValue ? src.TimeRelease.Value.ToString("d") : "Time Release not found",
                       NameAlbum: string.IsNullOrWhiteSpace(src.Album.AlbumName) ? "N/A"  : src.Album.AlbumName
                    ));

            CreateMap<ModelCardTrackDb, RDTOCardTrack>()
                .ConstructUsing(
                x => new RDTOCardTrack(
                    Name: x.Name,
                    Genre: (RDTOModel.Tracks.MusicGenre)x.MusicGenre,
                    Artist: x.Artist
                    )
                );
            #endregion

            CreateMap<DTOTrack, ModelTrackDB>().ConstructUsing(
                src => new ModelTrackDB
                {
                    ID = Guid.NewGuid(),
                    AlbumID = src.AlbumID,
                    Name = string.IsNullOrWhiteSpace(src.Name) ? "N?A" : src.Name,
                    Genre = (Postgresql.Models.MusicGenre)src.Genre,
                    SubArtist = string.IsNullOrWhiteSpace(src.SubArtist) ? "" : src.SubArtist,
                    TimeRelease = DateTime.Parse(!string.IsNullOrWhiteSpace(src.TimeRelease) ? src.TimeRelease : DateTime.MaxValue.ToString())  
                }
                );
        }
    }
}
