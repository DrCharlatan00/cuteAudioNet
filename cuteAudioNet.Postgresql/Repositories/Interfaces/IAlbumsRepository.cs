using cuteAudioNet.Postgresql.Models;
using System.Threading.Channels;

namespace cuteAudioNet.Postgresql.Repositories.Interfaces
{
    public interface IAlbumsRepository
    {
        Task<(Guid? ID, string Message)> CreateAsyncDb(ModelAlbumDB newModel);
        Task<IEnumerable<ModelAlbumDB>> GetAllAlbumDb();
        IAsyncEnumerable<ModelAlbumDB> GetAsyncEnumerableAllAlbumDb();
        Task<ModelAlbumDB?> GetByIdAsyncDb(Guid id);
        Task<IEnumerable<ModelAlbumDB>> GetWhisPaginationAsyncDb(int page, int pageSize);
        Task<string?> RemoveAsyncDb(Guid id);
        Task<(ModelAlbumDB? updateModel, string Message)> UpdateAsyncDb(ModelAlbumDB newModel);
    }
}