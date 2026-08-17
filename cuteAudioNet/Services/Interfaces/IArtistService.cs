using cuteAudioNet.APIModels.DTO.Artists;
using cuteAudioNet.APIModels.RDTOModel.Artists;

namespace cuteAudioNet.Services.Interfaces
{
    public interface IArtistService
    {
        Task<Guid> CreateAsync(DTOArtist artist);
        Task<RDTOArtist?> GetByIdAsync(Guid id);
        Task<IEnumerable<RDTOArtistCard>> GetCardArtistAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOArtistCard>> GetCardWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOArtist>> GetFullArtistAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOOnlyArtistInfo>> GetInfoArtistAsync(CancellationToken cancellationToken);
        Task<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RDTOArtist>> SearchByNIckNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOArtistCard>> SearchByNickNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOArtist> UpdateAsync(Guid id, DTOArtist artist);
    }
}