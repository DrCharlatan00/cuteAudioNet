using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class AlbumsRepository(PgContext context) : IAlbumsRepository
    {
        private readonly PgContext _context = context;

        #region Get
        public async Task<IEnumerable<ModelAlbumDB>> GetAllAlbumDb()
        {
            return await _context.albums.AsNoTracking().ToListAsync();
        }

        public async IAsyncEnumerable<ModelAlbumDB> GetAsyncEnumerableAllAlbumDb()
        {
            await foreach (var item in _context.albums.AsNoTracking().AsAsyncEnumerable())
            {
                yield return item;
            }
        }

        public async Task<ModelAlbumDB?> GetByIdAsyncDb(Guid id)
        {
            return await _context.albums.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<ModelAlbumDB>> GetWhisPaginationAsyncDb(int page, int pageSize)
        {
            return await _context.albums.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        #endregion

        #region Update
        public async Task<(ModelAlbumDB? updateModel, string Message)> UpdateAsyncDb(ModelAlbumDB newModel)
        {
            var old = await _context.albums.FirstOrDefaultAsync(x => x.ID == newModel.ID);
            if (old is null) return (null, "Not Found");
            try
            {
                if (newModel.ArtistID == Guid.Empty)
                {
                    var nw = new ModelAlbumDB
                    {
                        ID = old.ID,
                        ArtistID = old.ArtistID,
                        AlbumName = newModel.AlbumName ?? old.AlbumName,
                        Artist = old.Artist,
                        DateRelease = newModel.DateRelease ?? old.DateRelease
                    };

                    _context.Update(nw);
                    await _context.SaveChangesAsync();
                    return (nw, "Updated");
                }
                var art = await _context.artists.AsNoTracking().FirstOrDefaultAsync(x => x.ID == newModel.ID);
                var nwA = new ModelAlbumDB
                {
                    ID = old.ID,
                    ArtistID = art.ID,
                    AlbumName = newModel.AlbumName ?? old.AlbumName,
                    Artist = old.Artist,
                    DateRelease = newModel.DateRelease ?? old.DateRelease
                };
                return (nwA, "Updated");
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
                var Alb = await _context.albums.FirstOrDefaultAsync(x => x.ID == id);
                if (Alb is null) return "Not found";
                _context.Remove(Alb);
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
        public async Task<(Guid? ID, string Message)> CreateAsyncDb(ModelAlbumDB newModel)
        {
            newModel.ID = Guid.NewGuid();
            try
            {
                _context.Add(newModel);
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
