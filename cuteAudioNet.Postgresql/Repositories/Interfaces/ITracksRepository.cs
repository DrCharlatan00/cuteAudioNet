using cuteAudioNet.Postgresql.Models;
using System.Runtime.CompilerServices;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface ITracksRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelTrackDB newTrack);
        IAsyncEnumerable<ModelTrackDB> GetAllAsyncEnumerableDb();
        Task<IEnumerable<ModelTrackDB>> GetAllTrackAsyncDb();
        Task<ModelTrackDB?> GetByIDAsyncDb(Guid id);
        IAsyncEnumerable<ModelCardTrackDb> GetAllTrackCardAsyncEnumerableDb();
        Task<IEnumerable<ModelTrackDB>> GetOnlyTrackAsyncDb();
        Task<IEnumerable<ModelCardTrackDb>> GetWhisPaginationDb(int page, int pageSize);
        IAsyncEnumerable<ModelTrackDB> SearchByNameAsyncEnumerable(string name, CancellationToken cancellationToken);
        IAsyncEnumerable<ModelCardTrackDb> SearchByNameWithPaginationAsyncEnumerable(string name, int page, int pageSize,  CancellationToken cancellationToken);
        Task<string?> RemoveAsyncDb(Guid id);
        Task<(ModelTrackDB? UpdatedModel, string Message)> UpdateTracksAsyncDb(ModelTrackDB updatedTrack);
    }
}