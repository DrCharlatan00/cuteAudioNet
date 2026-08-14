using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface ITracksRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelTrackDB newTrack);
        IAsyncEnumerable<ModelTrackDB> GetAllAsyncEnumerableDb();
        Task<IEnumerable<ModelTrackDB>> GetAllTrackAsyncDb();
        Task<ModelTrackDB?> GetByIDAsyncDb(Guid id);
        IAsyncEnumerable<(string Name, MusicGenre Genre, string ArtistNickname)> GetAllTrackCardAsyncEnumerableDb();
        Task<IEnumerable<ModelTrackDB>> GetOnlyTrackAsyncDb();
        Task<IEnumerable<ModelTrackDB>> GetWhisPaginationDb(int page, int pageSize);
        Task<string?> RemoveAsyncDb(Guid id);
        Task<(ModelTrackDB? UpdatedModel, string Message)> UpdateTracksAsyncDb(ModelTrackDB updatedTrack);
    }
}