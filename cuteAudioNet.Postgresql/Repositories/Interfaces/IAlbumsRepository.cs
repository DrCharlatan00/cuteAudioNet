using cuteAudioNet.Postgresql.Models;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface IAlbumsRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelAlbumDB newModel);
        Task<IEnumerable<ModelAlbumDB>> GetAllAlbumDb();
        IAsyncEnumerable<ModelAlbumDB> GetAsyncEnumerableAllAlbumDb();
        Task<ModelAlbumDB?> GetByIdAsyncDb(Guid id);
        Task<IEnumerable<ModelAlbumDB>> GetOnlyAlbums();
        Task<IEnumerable<ModelAlbumCardDb>> GetWhisPaginationAsyncDb(int page, int pageSize);
        IAsyncEnumerable<ModelAlbumDB> SearchByNameAsyncEnumerable(string name,  CancellationToken cancellationToken);
        IAsyncEnumerable<ModelAlbumCardDb> SearchByNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<string?> RemoveAsyncDb(Guid id);
        Task<(ModelAlbumDB? updateModel, string Message)> UpdateAsyncDb(ModelAlbumDB newModel);
        IAsyncEnumerable<ModelAlbumCardDb> GetAsyncEnumerebleFromCardDb();
    }
}