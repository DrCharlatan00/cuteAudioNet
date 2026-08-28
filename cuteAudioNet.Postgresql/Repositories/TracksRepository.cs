using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace cuteAudioNet.Postgresql.Repositories
{
    public class TracksRepository(PgContext context) : ITracksRepository
    {

        private readonly PgContext _context = context;
        #region Get
        public async Task<IEnumerable<ModelTrackDB>> GetAllTrackAsyncDb()
        {
            return await _context.tracks.AsNoTracking().Include(x => x.Album).Include(x => x.Album.Artist).ToListAsync();
        }

        public async Task<IEnumerable<ModelTrackDB>> GetOnlyTrackAsyncDb()
        {
            return await _context.tracks.AsNoTracking().ToListAsync();
        }

        public async IAsyncEnumerable<ModelTrackDB> GetAllAsyncEnumerableDb()
        {
            await foreach (var data in _context.tracks.AsNoTracking().Include(x => x.Album).Include(x => x.Album.Artist).AsAsyncEnumerable())
            {
                yield return data;
            }

        }

        public async IAsyncEnumerable<ModelCardTrackDb> GetAllTrackCardAsyncEnumerableDb() {
            await foreach (var item in _context.tracks.AsNoTracking().Select(u => new ModelCardTrackDb
            (
                name: u.Name,
                musicGenre: u.Genre,
                artist: u.SubArtist != null ? u.Album.Artist.NickName + " , " + u.SubArtist : u.Album.Artist.NickName
            )
              
            ).AsAsyncEnumerable()) {
                yield return item;
            } 
        }


        public async Task<IEnumerable<ModelCardTrackDb>> GetWhisPaginationDb(int page, int pageSize)
        {
            return await _context.tracks
                .AsNoTracking()
                .OrderBy(x => x.ID)
                .Select(u => new ModelCardTrackDb
                    (
                    name: u.Name,
                    musicGenre: u.Genre,
                    artist: u.SubArtist != null ? u.Album.Artist.NickName + " , " + u.SubArtist : u.Album.Artist.NickName
                    )
                )
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ModelTrackDB?> GetByIDAsyncDb(Guid id)
        {
            return await _context.tracks.AsNoTracking().Include(x => x.Album).ThenInclude(x => x.Artist).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async IAsyncEnumerable<ModelTrackDB> SearchByNameAsyncEnumerable(string name, [EnumeratorCancellation] CancellationToken cancellationToken) {
            await foreach (var item in _context.tracks.AsNoTracking().Include(x => x.Album).Where(x => x.Name.Contains(name)).AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<ModelCardTrackDb> SearchByNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in _context.tracks
     .AsNoTracking()
     .Where(x => x.Name.Contains(name))
     .OrderBy(x => x.ID)
     .Skip((page - 1) * pageSize)
     .Take(pageSize)
      .Select(u => new ModelCardTrackDb
            (
                name: u.Name,
                musicGenre: u.Genre,
                artist: u.SubArtist != null ? u.Album.Artist.NickName + " , " + u.SubArtist : u.Album.Artist.NickName
            )
      )
     .AsAsyncEnumerable()
     .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }


        #endregion


        #region Update
        public async Task<(ModelTrackDB? UpdatedModel, string Message)> UpdateTracksAsyncDb(ModelTrackDB updatedTrack)
        {
            ArgumentException.ThrowIfNullOrEmpty(updatedTrack.ID.ToString());
            var old = await _context.tracks.FirstOrDefaultAsync(x => x.ID == updatedTrack.ID);

            if (old is null)
                return (null, "Track not found");

            var album = await _context.albums
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

            await _context.SaveChangesAsync();

            return (old, "Updated");
        }
        #endregion

        #region Remove

        public async Task<string?> RemoveAsyncDb(Guid id)
        {
            var rem = await _context.tracks.FirstOrDefaultAsync(x => x.ID == id);
            if (rem is null) return "Not found";
            try
            {
                _context.tracks.Remove(rem);
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

        public async Task<(Guid? ID, string Message)> CreateAsyncDb(ModelTrackDB newTrack)
        {
            try
            {
                newTrack.ID = Guid.NewGuid();
                await _context.tracks.AddAsync(newTrack);
                await _context.SaveChangesAsync();
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
