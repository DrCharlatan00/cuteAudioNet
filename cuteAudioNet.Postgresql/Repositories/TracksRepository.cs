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

        // !+ Add new method from card aswers
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

        public async IAsyncEnumerable<(string Name,MusicGenre Genre, string ArtistNickname)> GetAllTrackCardAsyncEnumerableDb() {
            await foreach (var item in  db.tracks.AsNoTracking().Select(u => new
            {
                u.Name,
                u.Genre,
                u.Album.Artist.NickName
            }).AsAsyncEnumerable()) {
                yield return (item.Name,item.Genre,item.NickName);
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

            if (old is null)
                return (null, "Track not found");

            var album = await db.albums
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == updatedTrack.AlbumID);

            old.Name = updatedTrack.Name ?? old.Name;
            old.SubArtist = updatedTrack.SubArtist ?? old.SubArtist;
            old.TimeRelease = updatedTrack.TimeRelease ?? old.TimeRelease;
            old.Genre = updatedTrack.Genre;

            if (album != null)
            {
                old.AlbumID = updatedTrack.AlbumID;
            }

            await db.SaveChangesAsync();

            return (old, "Updated");
        }
        #endregion

        #region Remove

        public async Task<string?> RemoveAsyncDb(Guid id)
        {
            var rem = await db.tracks.FirstOrDefaultAsync(x => x.ID == id);
            if (rem is null) return "Not found";
            try
            {
                db.tracks.Remove(rem);
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
