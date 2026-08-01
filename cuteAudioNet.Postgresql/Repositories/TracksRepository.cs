using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class TracksRepository(PgContext context) : ITracksRepository
    {

        // !+ Add new method from card aswers, use .Select(u => new {u.data!})
        private readonly PgContext db = context;
        #region Get
        public async Task<IEnumerable<ModelTrackDB>> GetAllTrackAsyncDb()
        {
            return await db.tracks.AsNoTracking().Include(x => x.Album).Include(x => x.Album.Artist).ToListAsync();
        }

        public async Task<IEnumerable<ModelTrackDB>> GetOnlyTrackAsyncDb()
        {
            return await db.tracks.AsNoTracking().ToListAsync();
        }

        public async IAsyncEnumerable<ModelTrackDB> GetAllAsyncEnumerableDb()
        {
            await foreach (var data in db.tracks.AsNoTracking().Include(x => x.Album).Include(x => x.Album.Artist).AsAsyncEnumerable())
            {
                yield return data;
            }

        }




        public async Task<IEnumerable<ModelTrackDB>> GetWhisPaginationDb(int page, int pageSize)
        {
            return await db.tracks
                .Include(x => x.Album)
                .Include(x => x.Album.Artist)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ModelTrackDB?> GetByIDAsyncDb(Guid id)
        {
            return await db.tracks.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }


        #endregion


        #region Update
        public async Task<(ModelTrackDB? UpdatedModel, string Message)> UpdateTracksAsyncDb(ModelTrackDB updatedTrack)
        {
            ArgumentException.ThrowIfNullOrEmpty(updatedTrack.ID.ToString());
            var old = await db.tracks.FirstOrDefaultAsync(x => x.ID == updatedTrack.ID);
            var AlbumID = await db.albums.AsNoTracking()
                                           .FirstOrDefaultAsync(x => x.ID == updatedTrack.AlbumID);
            try
            {
                if (AlbumID is null || AlbumID.ID == Guid.Empty)
                {
                    var trackNew = new ModelTrackDB
                    {
                        ID = old.ID,
                        Album = updatedTrack.Album ?? old.Album,
                        AlbumID = old.AlbumID,
                        Genre = updatedTrack.Genre,
                        Name = updatedTrack.Name ?? old.Name,
                        SubArtist = updatedTrack.SubArtist ?? old.SubArtist,
                        TimeRelease = updatedTrack.TimeRelease ?? old.TimeRelease,
                    };
                    db.Update(trackNew);
                    await db.SaveChangesAsync();
                    return (trackNew, "Updated");
                }
                var trackNewold = new ModelTrackDB
                {
                    ID = old.ID,
                    Album = updatedTrack.Album ?? old.Album,
                    AlbumID = updatedTrack.AlbumID,
                    Genre = updatedTrack.Genre,
                    Name = updatedTrack.Name ?? old.Name,
                    SubArtist = updatedTrack.SubArtist ?? old.SubArtist,
                    TimeRelease = updatedTrack.TimeRelease ?? old.TimeRelease,
                };
                db.Update(trackNewold);
                await db.SaveChangesAsync();
                return (trackNewold, "Updated");
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
            var rem = await db.tracks.FirstOrDefaultAsync(x => x.ID == id);
            if (rem is null) return "Not found";
            try
            {
                db.Remove(rem);
                await db.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        #endregion

        #region Create

        public async Task<(Guid? ID, string Message)> CreateAsyncDb(ModelTrackDB newTrack)
        {
            try
            {
                newTrack.ID = Guid.NewGuid();
                await db.tracks.AddAsync(newTrack);
                await db.SaveChangesAsync();
                return (newTrack.ID, "Created");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }


        }

        #endregion

    }
}
