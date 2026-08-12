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
        Task<RDTOTrack?> GetByIDAsync(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<ModelTrackDB> UpdateAsync(Guid id, DTOTrack dto);
    }
}