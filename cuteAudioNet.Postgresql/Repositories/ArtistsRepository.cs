using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class ArtistsRepository(PgContext context) : IArtistsRepository
    {
        private readonly PgContext _context = context;

        #region Get
       
        public async Task<IEnumerable<ModelArtistDB>> GetAllArtistDb(CancellationToken cancellationToken)
        {
            return await _context.artists.Include(x => x.Albums).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async IAsyncEnumerable<ModelArtistDB> GetAsyncEnumerableAllArtistDb([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in _context.artists.Include(x => x.Albums).AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<ModelArtistCardDb> GetWithCardArtistsAsyncEnumertable([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in _context.artists.AsNoTracking().Select(u => new ModelArtistCardDb( u.NickName, u.Albums )).AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<ModelArtistDB?> GetByIdAsyncDb(Guid id)
        {
            return await _context.artists.Include(x => x.Albums).AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<ModelArtistDB>> GetOnlyArtistsDb(CancellationToken cancellationToken)
        {
            return await _context.artists.AsNoTracking().ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<ModelArtistCardDb>> GetWithPaginationAsyncDB(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.artists
                .AsNoTracking()
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ModelArtistCardDb(u.NickName, u.Albums))
                .ToListAsync(cancellationToken)
                ;
        }

        public async IAsyncEnumerable<ModelArtistDB> SearchByNickNameAsyncEnumerable(string name, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in _context.artists.AsNoTracking().Include(x =>x.Albums).Where(x => x.NickName == name).AsAsyncEnumerable())
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<ModelArtistCardDb> SearchByNickNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in _context.artists.AsNoTracking().Where(x => x.NickName == name).Skip((page - 1) * pageSize).Take(pageSize).Select(u => new ModelArtistCardDb(u.NickName, u.Albums))
.ToAsyncEnumerable())
            {
                yield return item;
            }
        }
        #endregion


        #region Update
        public async Task<(ModelArtistDB? updateModel, string Message)> UpdateAsyncDb(ModelArtistDB newModel)
        {
            var old = await _context.artists.FirstOrDefaultAsync(x => x.ID == newModel.ID);
            if (old is null) return (null, "Not Found");
            try
            {
                old.ArtistName = !string.IsNullOrWhiteSpace(newModel.ArtistName) ? newModel.ArtistName : old.ArtistName;
                old.NickName = !string.IsNullOrWhiteSpace(newModel.NickName) ? newModel.NickName : old.NickName;
                old.Surname = !string.IsNullOrWhiteSpace(newModel.Surname) ? newModel.Surname : old.Surname;
                old.BornDate = newModel.BornDate ?? old.BornDate;
                old.Pathonymic = !string.IsNullOrWhiteSpace(newModel.Pathonymic) ? newModel.Pathonymic : old.Pathonymic;
                await _context.SaveChangesAsync();
                return (old, "Updated");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }


        }
        #endregion

        #region Remove
        public async Task<string?> RemoveAsyncDb(Guid id)
        {
            try
            {
                var Alb = await _context.artists.FirstOrDefaultAsync(x => x.ID == id);
                if (Alb is null) return "Not found";
                _context.artists.Remove(Alb);
                await _context.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Create
        public async Task<(Guid? ID, string Message)> CreateAsyncDb(ModelArtistDB newModel)
        {
            newModel.ID = Guid.NewGuid();
            try
            {
                await _context.artists.AddAsync(newModel);
                await _context.SaveChangesAsync();
                return (newModel.ID, "Created");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        #endregion

    }
}
