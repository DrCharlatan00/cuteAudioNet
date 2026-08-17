using AutoMapper;
using cuteAudioNet.APIModels.DTO.Artists;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Artists;
using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.APIModels.Mapping
{
    public class MappingArtistProfile : Profile
    {
        public MappingArtistProfile()
        {
            CreateMap<ModelArtistDB, RDTOArtist>().ConstructUsing(
                    x => new RDTOArtist(
                        Name: x.ArtistName ?? "N/A",
                        NickName: x.NickName ?? "N/A",
                        Surname: x.Surname ?? "N/A",
                        BordDate: x.BornDate,
                        Pathonymic: x.Pathonymic ?? "N/A",
                        null
                        )
                );
            CreateMap<ModelArtistDB, RDTOOnlyArtistInfo>().ConstructUsing(
                x => new RDTOOnlyArtistInfo(
                        Name: x.ArtistName ?? "N/A",
                        NickName: x.NickName ?? "N/A",
                        Surname: x.Surname ?? "N/A",
                        BordDate: x.BornDate,
                        Pathonymic: x.Pathonymic ?? "N/A"
                    )
                );
            CreateMap<ModelArtistDB, RDTOArtistCard>().ConstructUsing(
                x => new RDTOArtistCard(
                    NickName: x.NickName ?? "N/A",
                    null
                    )
                );
           
        }
    }
}
