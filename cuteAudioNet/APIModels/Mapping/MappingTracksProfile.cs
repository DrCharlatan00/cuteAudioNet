using AutoMapper;
using cuteAudioNet.APIModels.DTO;
using cuteAudioNet.APIModels.RDTOModel;
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
                    Genre: (RDTOModel.MusicGenre)src.Genre,
                    src.Album.Artist.NickName + "feat." + src.SubArtist
                    ));
            CreateMap<ModelTrackDB, RDTOTrack>()
                .ConstructUsing(src => new RDTOTrack(
                       Name: src.Name,
                       Artists: src.Album.Artist.NickName + "feat." + src.SubArtist,
                       Genre: (RDTOModel.MusicGenre)src.Genre,
                       TimeRelease: src.TimeRelease.HasValue ? src.TimeRelease.Value.ToString("d") : "Time Release not found",
                       NameAlbum: src.Album.AlbumName ?? "Album Not Found"
                    ));
            #endregion

            CreateMap<DTOTrack, ModelTrackDB>().ConstructUsing(
                src => new ModelTrackDB
                {
                    ID = Guid.NewGuid(),
                    AlbumID = src.AlbumID,
                    Name = src.Name ?? "N?A",
                    Genre = (Postgresql.Models.MusicGenre)src.Genre,
                    SubArtist = src.SubArtist ?? "",
                    TimeRelease = DateTime.Parse(!string.IsNullOrWhiteSpace(src.TimeRelease) ? src.TimeRelease : DateTime.MaxValue.ToString())  
                }
                );
        }
    }
}
