using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class ArtistsRepository(PgContext context) : IArtistsRepository
    {
        private readonly PgContext _context = context;

        #region Get
        public async Task<IEnumerable<ModelArtistDB>> GetAllArtistDb()
        {
            return await _context.artists.Include(x => x.Albums).AsNoTracking().ToListAsync();
        }

        public async IAsyncEnumerable<ModelArtistDB> GetAsyncEnumerableAllArtistDb()
        {
            await foreach (var item in _context.artists.Include(x => x.Albums).AsNoTracking().AsAsyncEnumerable())
            {
                yield return item;
            }
        }

        public async Task<ModelArtistDB?> GetByIdAsyncDb(Guid id)
        {
            return await _context.artists.Include(x => x.Albums).AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<ModelArtistDB>> GetAllWhisPaginationAsyncDb(int page, int pageSize)
        {
            return await _context.artists.AsNoTracking()
                .Include(x => x.Albums)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelArtistDB>> GetOnlyArtistsDb()
        {
            return await _context.artists.AsNoTracking().ToListAsync();
        }
        #endregion


        #region Update
        public async Task<(ModelArtistDB? updateModel, string Message)> UpdateAsyncDb(ModelArtistDB newModel)
        {
            var old = await _context.artists.FirstOrDefaultAsync(x => x.ID == newModel.ID);
            if (old is null) return (null, "Not Found");
            try
            {
                var art = new ModelArtistDB
                {
                    ID = old.ID,
                    ArtistName = newModel.ArtistName ?? old.ArtistName,
                    NickName = newModel.NickName ?? old.NickName,
                    Albums = newModel.Albums ?? old.Albums,
                    BornDate = newModel.BornDate ?? old.BornDate,
                    Pathonymic = newModel.Pathonymic ?? old.Pathonymic,
                    Surname = newModel.Surname ?? old.Surname,
                };
                await _context.SaveChangesAsync();
                return (art, "Updated");
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
