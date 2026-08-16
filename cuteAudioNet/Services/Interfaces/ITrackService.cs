using cuteAudioNet.APIModels.DTO;
using cuteAudioNet.APIModels.RDTOModel;
using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.Services.Interfaces
{
    public interface ITrackService
    {
        Task<Guid> CreateAsync(DTOTrack newTrack);
        Task<IEnumerable<RDTOTrack>> GetAllTrackAsync();
        Task<IEnumerable<RDTOCardTrack>> GetTrackCardAsync();
        Task<ModelTrackDB?> GetWhisID(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<ModelTrackDB> Update(Guid id, DTOTrack dto);
    }
}