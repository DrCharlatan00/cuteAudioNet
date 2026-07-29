using AutoMapper;
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
                            DateRelease: src.DateRelease.Value.ToString("d") ?? "No time release",
                            ArtistName: src.Artist.NickName,
                            default
                        )
                );
        }
    }
}
