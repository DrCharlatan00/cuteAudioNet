using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface IArtistsRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelArtistDB newModel);
        Task<IEnumerable<ModelArtistDB>> GetAllArtistDb();
        Task<IEnumerable<ModelArtistDB>> GetAllWhisPaginationAsyncDb(int page, int pageSize);
        IAsyncEnumerable<ModelArtistDB> GetAsyncEnumerableAllArtistDb();
        Task<ModelArtistDB?> GetByIdAsyncDb(Guid id);
        Task<IEnumerable<ModelArtistDB>> GetOnlyArtistsDb();
        Task<string?> RemoveAsyncDb(Guid id);
        Task<(ModelArtistDB? updateModel, string Message)> UpdateAsyncDb(ModelArtistDB newModel);
    }
}