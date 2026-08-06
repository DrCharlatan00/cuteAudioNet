using AutoMapper;
using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.APIModels.Mapping
{
    public class MappingAlbumProfile : Profile
    {
        public MappingAlbumProfile()
        {
            CreateMap<ModelAlbumDB, RDTOAlbumCard>().ConstructUsing(
                    src => new RDTOAlbumCard(
                        Name: src.AlbumName,
                        ArtistName: src.Artist.NickName
                        )
                );
            CreateMap<ModelAlbumDB, RDTOAlbum>().ConstructUsing(
                    src => new RDTOAlbum(
                            Name: src.AlbumName,
                            DateRelease: src.DateRelease.HasValue ? src.DateRelease.Value.ToShortTimeString() : "No time",
                            ArtistName: src.Artist.NickName,
                            default
                        )
                );
            CreateMap<DTOUpdateAlbum, ModelAlbumDB>().ConstructUsing(
                    src => new ModelAlbumDB
                    {
                        ID = src.id,
                        AlbumName = src.AlbumName,
                        DateRelease = !string.IsNullOrWhiteSpace(src.DateRelease)
                ? DateTime.Parse(src.DateRelease)
                : DateTime.MinValue // I don't know why he don't like null
                    }
                );
            CreateMap<DTOCreateAlbum, ModelAlbumDB>().ConstructUsing(
                    src => new ModelAlbumDB { 
                        ID = Guid.NewGuid(),
                        AlbumName = src.Name,
                        DateRelease = 
                        !string.IsNullOrWhiteSpace(src.DateRelease) 
                        ? DateTime.Parse(src.DateRelease) 
                        : null,
                        ArtistID = src.IdArtist
                    }
                );
        }
    }
}
