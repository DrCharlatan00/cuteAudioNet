using cuteAudioNet.Postgresql.Models;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface IArtistsRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelArtistDB newModel);
        Task<IEnumerable<ModelArtistDB>> GetAllArtistDb(CancellationToken cancellationToken);
        IAsyncEnumerable<ModelArtistDB> GetAsyncEnumerableAllArtistDb(CancellationToken cancellationToken);
        Task<ModelArtistDB?> GetByIdAsyncDb(Guid id);
        Task<IEnumerable<ModelArtistDB>> GetOnlyArtistsDb(CancellationToken cancellationToken);
        IAsyncEnumerable<ModelArtistCardDb> GetWithCardArtistsAsyncEnumertable(CancellationToken cancellationToken);
        Task<IEnumerable<ModelArtistCardDb>> GetWithPaginationAsyncDB(int page, int pageSize, CancellationToken cancellationToken);
        Task<string?> RemoveAsyncDb(Guid id);
        IAsyncEnumerable<ModelArtistDB> SearchByNickNameAsyncEnumerable(string name, CancellationToken cancellationToken);
        IAsyncEnumerable<ModelArtistCardDb> SearchByNickNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<(ModelArtistDB? updateModel, string Message)> UpdateAsyncDb(ModelArtistDB newModel);
    }
}