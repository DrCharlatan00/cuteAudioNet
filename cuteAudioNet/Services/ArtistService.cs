using AutoMapper;
using cuteAudioNet.APIModels.DTO.Artists;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Artists;
using cuteAudioNet.Exceptions;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services.Caching;
using cuteAudioNet.Services.Interfaces;
using FluentValidation;
using StackExchange.Redis;
using System.Data;


namespace cuteAudioNet.Services
{
    public class ArtistService(
        IArtistsRepository repository,
        IValidator<DTOArtist> validator,
        ILogger<ArtistService> logger,
        ICacheService cache,
        IMapper mapper
        ) : IArtistService
    {
        private readonly IArtistsRepository repository = repository;
        private readonly IValidator<DTOArtist> validator = validator;
        private readonly ILogger<ArtistService> logger = logger;
        private readonly ICacheService cache = cache;
        private readonly IMapper mapper = mapper;

        #region Get
        public async Task<IEnumerable<RDTOArtist>> GetFullArtistAsync(CancellationToken cancellationToken)
        {
            List<RDTOArtist> data = new List<RDTOArtist>();
            await foreach (var item in repository.GetAsyncEnumerableAllArtistDb(cancellationToken))
            {
                data.Add(MapFull(item));
            }
            return data;
        }

        public async Task<IEnumerable<RDTOArtistCard>> GetCardArtistAsync(CancellationToken cancellationToken)
        {
            List<RDTOArtistCard> data = new();
            await foreach (var item in repository.GetWithCardArtistsAsyncEnumertable(cancellationToken))
            {
                List<RDTOAlbumCard> albumCards = new List<RDTOAlbumCard>();
                foreach (var album in item.album)
                {
                    albumCards.Add(MapAlbumCard(album));
                }
                data.Add(new RDTOArtistCard(item.NickName, albumCards));
            }
            return data;
        }

        public async Task<IEnumerable<RDTOOnlyArtistInfo>> GetInfoArtistAsync(CancellationToken cancellationToken)
        {
            var data = await repository.GetOnlyArtistsDb(cancellationToken);
            if (data is null)
            {
                logger.LogError("Collection {collection} returns in db with null in func {func}", nameof(RDTOOnlyArtistInfo), nameof(GetInfoArtistAsync));
                throw new DbGetCollectionIsNull("Collection information is null", nameof(RDTOOnlyArtistInfo), nameof(GetInfoArtistAsync));
            }
            return data.Select(MapInfo).ToList();
        }

        public async Task<RDTOArtist?> GetByIdAsync(Guid id)
        {
            ModelArtistDB? item = await repository.GetByIdAsyncDb(id);
            return item is not null ? MapFull(item) : null;
        }

        public async Task<IEnumerable<RDTOArtistCard>> GetCardWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            const string cacheVerKey = "artist:version";

            long version = await cache.GetVersionAsync(cacheVerKey);

            string cacheKey = $"artist:card:v{version}:page{page}:size{pageSize}";

            var cacheData = await cache.GetAsync<IEnumerable<RDTOArtistCard>>(cacheKey);

            if (cacheData is not null) return cacheData;

            var data = await repository.GetWithPaginationAsyncDB(page, pageSize, cancellationToken);
            List<RDTOArtistCard> rdto = new();
            foreach (var album in data)
            {
                List<RDTOAlbumCard> albumCards = new List<RDTOAlbumCard>();
                foreach (var item in album.Albums)
                {
                    albumCards.Add(MapAlbumCard(item));
                }
                rdto.Add(new RDTOArtistCard(album.NickName, albumCards));
            }
            await cache.SetAsync<IEnumerable<RDTOArtistCard>>(cacheKey, rdto, TimeSpan.FromMinutes(2));
            return rdto;
        }

        public async Task<IEnumerable<RDTOArtist>> SearchByNIckNameAsync(string name, CancellationToken cancellationToken)
        {
            List<RDTOArtist> rdto = new();
            await foreach (var item in repository.SearchByNickNameAsyncEnumerable(name, cancellationToken))
            {
                List<RDTOAlbum> albums = new List<RDTOAlbum>();
                foreach (var album in item.Albums)
                {
                    albums.Add(MapAlbum(album));
                }
                RDTOArtist artist = MapFull(item);
                artist.Albums = albums;
                rdto.Add(artist);
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOArtistCard>> SearchByNickNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            List<RDTOArtistCard> rdto = new();
            await foreach (var item in repository.SearchByNickNameWithPaginationAsyncEnumerable(name, page, pageSize, cancellationToken))
            {
                rdto.Add(new RDTOArtistCard(
                    item.NickName,
                    item.Albums.Select(MapAlbumCard).ToList()
                    )
                    );
            }
            return rdto;
        }

        #endregion

        #region Update
        public async Task<RDTOArtist> UpdateAsync(Guid id, DTOArtist artist)
        {
            ArgumentNullException.ThrowIfNull(artist);
            await validator.ValidateAndThrowAsync(artist);
            ModelArtistDB artistDB;
            if (DateTime.TryParse(artist.DateTime, out DateTime BornTime))
            {
                ModelArtistDB model = new ModelArtistDB
                {
                    ID = id,
                    ArtistName = artist.Name,
                    NickName = artist.NickName,
                    BornDate = BornTime,
                    Pathonymic = artist.Pathonymic,
                    Surname = artist.Surname
                };
                artistDB = model;
            }
            else
            {
                ModelArtistDB model = new ModelArtistDB
                {
                    ID = id,
                    ArtistName = artist.Name,
                    NickName = artist.NickName,
                    BornDate = null,
                    Pathonymic = artist.Pathonymic,
                    Surname = artist.Surname
                };
                artistDB = model;
            }


            var item = await repository.UpdateAsyncDb(artistDB);
            if (item.updateModel is not null)
            {
                await cache.IncrementAsync("artist:version");
                return MapFull(item.updateModel);
            }
            logger.LogError("Update operation is failed\nerror: {error} \nModel: {@model}", item.Message, artist);
            throw new UpdateItemBaseFail<ArtistService, ModelArtistDB>(item.Message);

        }

        #endregion

        #region Create 
        public async Task<Guid> CreateAsync(DTOArtist artist)
        {
            ArgumentNullException.ThrowIfNull(artist);
            await validator.ValidateAndThrowAsync(artist);
            ModelArtistDB artistDB;
            if (DateTime.TryParse(artist.DateTime, out DateTime BornTime))
            {
                ModelArtistDB model = new ModelArtistDB
                {
                    ArtistName = artist.Name,
                    NickName = artist.NickName,
                    BornDate = BornTime,
                    Pathonymic = artist.Pathonymic,
                    Surname = artist.Surname
                };
                artistDB = model;
            }
            else
            {
                ModelArtistDB model = new ModelArtistDB
                {
                    ArtistName = artist.Name,
                    NickName = artist.NickName,
                    BornDate = null,
                    Pathonymic = artist.Pathonymic,
                    Surname = artist.Surname
                };
                artistDB = model;
            }
            var result = await repository.CreateAsyncDb(artistDB);
            if (result.ID is not null)
            {
                await cache.IncrementAsync("artist:version");
                return (Guid)result.ID;

            }
            logger.LogError("Create is failed, Information \nerror: {error} \nModel: {@model}", result.Message, artist);
            throw new CreateItemBaseFail<ArtistService, ModelArtistDB>("Create if failed");
        }
        #endregion

        #region Remove
        public async Task<bool> RemoveAsync(Guid id)
        {
            var result = await repository.RemoveAsyncDb(id);
            if (result is null)
            {
                await cache.IncrementAsync("artist:version");
                return true;
            }
            logger.LogError("Remove is failed, info error: {error} , Guid {id}", result, id);
            throw new RemoveItemBaseFail<ArtistService, ModelArtistDB>("Remove if failed");
        }
        #endregion


        #region Mappers
        private RDTOArtist MapFull(ModelArtistDB model) => mapper.Map<RDTOArtist>(model);
        private RDTOOnlyArtistInfo MapInfo(ModelArtistDB model) => mapper.Map<RDTOOnlyArtistInfo>(model);
        private RDTOAlbumCard MapAlbumCard(ModelAlbumDB model) {
            try
            {
                return mapper.Map<RDTOAlbumCard>(model);
            }
            catch {
                return new RDTOAlbumCard(model.AlbumName,null);
            }
        }
        private RDTOAlbum MapAlbum(ModelAlbumDB model) => mapper.Map<RDTOAlbum>(model);
        #endregion
    }
}
