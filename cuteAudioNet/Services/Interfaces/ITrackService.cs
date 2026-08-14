using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.Services.Interfaces
{
    public interface ITrackService
    {
        Task<Guid> CreateAsync(DTOTrack newTrack);
        Task<IEnumerable<RDTOTrack>> GetAllTrackAsync();
        Task<IEnumerable<RDTOCardTrack>> GetTrackCardAsync();
        Task<IEnumerable<RDTOCardTrack>> GetByPaginationCardAsync(int page, int pageSize);
        Task<IEnumerable<RDTOTrack>> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOCardTrack>> SearchByNamePaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOTrack?> GetByIDAsync(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<ModelTrackDB> UpdateAsync(Guid id, DTOTrack dto);
    }
}