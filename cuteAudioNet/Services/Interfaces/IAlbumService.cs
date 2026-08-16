using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;

namespace cuteAudioNet.Services.Interfaces
{
    public interface IAlbumService
    {
        Task<Guid> CreateItemAlbum(DTOCreateAlbum album);
        Task<IEnumerable<RDTOAlbumCard>> GetAllFromCardAsync();
        Task<RDTOAlbum?> GetByIDAsync(Guid id);
        Task<IEnumerable<RDTOAlbumCard>> GetByPaginationCard(int page, int pageSize);
        Task<IEnumerable<RDTOAlbum>> GetFullInfomaionAlbumAsync();
        Task<IEnumerable<RDTOAlbum>> SearchByNameAsync(string name, CancellationToken cnt);
        Task<IEnumerable<RDTOAlbumCard>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cnt);
        Task<bool> RemoveItemAlbum(Guid id);
        Task<RDTOAlbum> UpdateItemAlbum(DTOUpdateAlbum model);
    }
}