using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class AlbumsRepository(PgContext context) : IAlbumsRepository
    {
        private readonly PgContext _context = context;

#warning Review class
        #region Get
        /// <summary> Get full information Album </summary>
        /// <returns> IEnumerable collection Db model Album </returns>
        /// <remarks> for performance use method GetAsyncEnumerableAllAlbumDb()</remarks>
        public async Task<IEnumerable<ModelAlbumDB>> GetAllAlbumDb()
        {
            return await _context.albums.AsNoTracking().Include(x => x.Tracks).Include(x => x.Artist).ToListAsync();
        }

        /// <summary>
        /// method async returns full albums collection
        /// </summary>
        /// <returns>IAsyncEnumerable Model db album, async return one item in collection use await forearch</returns>
        
        public async IAsyncEnumerable<ModelAlbumDB> GetAsyncEnumerableAllAlbumDb()
        {
            await foreach (var item in _context.albums.AsNoTracking().Include(x => x.Tracks).Include(x => x.Artist).AsAsyncEnumerable())
            {
                yield return item;
            }
        }
/// <summary>
/// Method async return Model album for card
/// </summary>
/// <returns>return async (string AlbumName, string ArtistNickname) for card model</returns>
        public async IAsyncEnumerable<(string AlbumName,string ArtistNickname)> GetAsyncEnumerebleFromCardDb() {
            await foreach (var item in _context.albums.AsNoTracking().Select(u => new { u.AlbumName, u.Artist.NickName }).AsAsyncEnumerable()) {
                yield return (item.AlbumName,item.NickName);
            }
        }
/// <summary>
/// Method return only information for album 
/// </summary>
/// <returns>IEnumerable collection Model db album with only information album without track and artist info</returns>
/// <remarks>Do not forgot. return info do not return data for track and artist information</remarks>
        public async Task<IEnumerable<ModelAlbumDB>> GetOnlyAlbums() {
            return await _context.albums.AsNoTracking().ToListAsync();
        }
/// <summary>
/// Return search and return full album by ID
/// </summary>
/// <param name="id">Guid id album for db</param>
/// <returns>Full model db album or null if item not found</returns>
        public async Task<ModelAlbumDB?> GetByIdAsyncDb(Guid id)
        {
            return await _context.albums.AsNoTracking().Include(x => x.Tracks).Include(x => x.Artist).FirstOrDefaultAsync(x => x.ID == id);
        }
/// <summary>
/// get card album with pagination
/// </summary>
/// <param name="page">current number page</param>
/// <param name="pageSize">count items</param>
/// <returns>IEnumerable collection model db album </returns>
        public async Task<IEnumerable<ModelAlbumDB>> GetWhisPaginationAsyncDb(int page, int pageSize)
        {
            return await _context.albums.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        #endregion

        #region Update
        /// <summary>
        /// Update in db album
        /// </summary>
        /// <param name="newModel">Model db album, do not forgot write to id in model </param>
        /// <returns>(Model db album with artist, message result operations )</returns>
        /// <remarks>Do not forgot fill out id var</remarks>
        public async Task<(ModelAlbumDB? updateModel, string Message)> UpdateAsyncDb(ModelAlbumDB newModel)
        {
            var old = await _context.albums.Include(x => x.Artist).FirstOrDefaultAsync(x => x.ID == newModel.ID);
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
                await _context.SaveChangesAsync();
                return (nwA, "Updated");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }


        }
        #endregion


        
        #region Remove
        /// <summary>
        /// Remove item album in db
        /// </summary>
        /// <param name="id">ID item which you want remove</param>
        /// <returns>result operation string</returns>
        public async Task<string?> RemoveAsyncDb(Guid id)
        {
            try
            {
                var Alb = await _context.albums.FirstOrDefaultAsync(x => x.ID == id);
                if (Alb is null) return "Not found";
                _context.albums.Remove(Alb);
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
        
        /// <summary>
        /// Create item album in db 
        /// </summary>
        /// <param name="newModel">Model db item, not required id, id creating in func</param>
        /// <returns></returns>
        public async Task<(Guid? ID, string Message)> CreateAsyncDb(ModelAlbumDB newModel)
        {
            newModel.ID = Guid.NewGuid();
            try
            {
                await _context.albums.AddAsync(newModel);
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
