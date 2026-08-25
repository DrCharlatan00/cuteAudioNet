using AutoMapper;
using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.APIModels.Validators;
using cuteAudioNet.Exceptions;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services.Caching;
using cuteAudioNet.Services.Interfaces;
using cuteAudioNet.SignalRHubs;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace cuteAudioNet.Services
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    public class AlbumService(
#pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена


        IAlbumsRepository repository,
        ILogger<AlbumService> logger,
        IMapper mapper,
        IValidator<DTOCreateAlbum> createValidator,
        IValidator<DTOUpdateAlbum> updateValidator,
        ICacheService cache,
        IHubContext<AlbumsHub> hubContext
        ) : IAlbumService
    {
        private readonly IAlbumsRepository repository = repository;
        private readonly ILogger<AlbumService> logger = logger;
        private readonly IMapper mapper = mapper;
        private readonly IValidator<DTOCreateAlbum> createValidator = createValidator;
        private readonly IValidator<DTOUpdateAlbum> updateValidator = updateValidator;
        private readonly ICacheService cache = cache;
        private readonly IHubContext<AlbumsHub> hubContext = hubContext;

        #region Get 
        /// <summary>
        /// Get Albums whis Card 
        /// </summary>
        /// <returns>Collections Albums Card</returns>
        /// <remarks>This not tested code,very possible his not work </remarks>


        public async Task<IEnumerable<RDTOAlbumCard>> GetAllFromCardAsync()
        {

            List<RDTOAlbumCard> cards = new();
            
            await foreach ((string AlbumName, string ArtistNickname) data in repository.GetAsyncEnumerebleFromCardDb())
            {
                cards.Add(new RDTOAlbumCard(data.AlbumName, data.ArtistNickname));
            }
  
            return cards;
        }

        /// <summary>
        ///  Get Albums whis artist and track
        /// </summary>
        /// <returns> collection is Albums whis tracks</returns>
        /// <exception cref="DbGetCollectionIsNull"></exception>
        /// <remarks>This not tested code,very possible his not work </remarks>

        public async Task<IEnumerable<RDTOAlbum>> GetFullInfomaionAlbumAsync()
        {
           
            List<RDTOAlbum> data = new List<RDTOAlbum>();
            await foreach (var item in repository.GetAsyncEnumerableAllAlbumDb())
            {
                if (item is null) throw new DbGetCollectionIsNull("In await foreach", nameof(ModelAlbumDB), nameof(RDTOAlbum));
                IEnumerable<ModelTrackDB> tracks = item.Tracks;
                var MappedTrack = tracks.Select(MapTrack).ToList();
                RDTOAlbum album = MapFull(item);
                data.Add(new RDTOAlbum(
                        Name: album.Name,
                        DateRelease: album.DateRelease,
                        ArtistName: album.ArtistName,
                        Tracks: MappedTrack
                    ));
            }
            return data;

        }
        /// <summary>
        /// Get album by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>RDTO model Album whis track and artist</returns>
        public async Task<RDTOAlbum?> GetByIDAsync(Guid id)
        {
            ModelAlbumDB? item = await repository.GetByIdAsyncDb(id);
            if (item is null) return null;
            return MapFull(item);
        }


        /// <summary>
        /// Get by pagination card 
        /// </summary>
        /// <param name="page">page in site</param>
        /// <param name="pageSize">count elements for site</param>
        /// <returns>Collection card RDTOAlbumCard </returns>
        /// <exception cref="DbGetCollectionIsNull">  possible if db return null</exception>
        public async Task<IEnumerable<RDTOAlbumCard>> GetByPaginationCard(int page, int pageSize)
        {
            if (page <= 0 || page > 10000)
            {
                throw new ArgumentException("Page is bad");
            }

            if (pageSize <= 0 || pageSize > 10000)
            {
                throw new ArgumentException("Page size is bad");
            }

            const string cacheVersion = "albums:version";

            var version =  await cache.GetVersionAsync(cacheVersion);

            string cacheKey = $"albums:card:v{1}:page{page}:size:{pageSize}";
            var cacheData = await cache.GetAsync<IEnumerable<RDTOAlbumCard>>(cacheKey);

            if (cacheData is not null) {
                return cacheData;
            }

            var data = await repository.GetWhisPaginationAsyncDb(page, pageSize);
            if (data is null) throw new DbGetCollectionIsNull(null, nameof(ModelAlbumDB), nameof(GetByPaginationCard));
            await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(2));
            return data.Select(Map).ToImmutableList();
        }

        /// <summary>
        /// Search by name
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <returns>collection items </returns>
        public async Task<IEnumerable<RDTOAlbum>> SearchByNameAsync(string name,CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is bad or null");

            List<RDTOAlbum> albums = new List<RDTOAlbum>();
            await foreach (var item in repository.SearchByNameAsyncEnumerable(name,cancellationToken).WithCancellation(cancellationToken))
            {
                albums.Add(MapFull(item));
            }
            return albums;

        }

        /// <summary>
        /// Search by name with pagination 
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="page">current page</param>
        /// <param name="pageSize">count items</param>
        /// <returns>collection paged card items</returns>
        /// <exception cref="ArgumentException">if page or pageSize is have a bad data</exception>
        public async Task<IEnumerable<RDTOAlbumCard>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is bad or null");

            if (page <= 0 || page > 10000) {
                throw new ArgumentException("Page is bad");
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                throw new ArgumentException("Page size is bad");
            }

            List<RDTOAlbumCard> albums = new List<RDTOAlbumCard>();
            await foreach (var item in repository.SearchByNameWithPaginationAsyncEnumerable(name,page,pageSize,cancellationToken).WithCancellation(cancellationToken))
            {
                albums.Add(Map(item));
            }
            return albums;

        }

        #endregion

        #region Update

        /// <summary>
        /// Update album method
        /// </summary>
        /// <param name="model">update model, not forgot set ID</param>
        /// <returns>Updated Album</returns>
        /// <exception cref="UpdateItemBaseFail{AlbumService, DTOUpdateAlbum}"></exception>
        /// <exception cref="ValidationException">if validate is fall</exception>
        /// <exception cref="ArgumentNullException">if get DTOUpdateAlbum is null </exception>

        public async Task<RDTOAlbum> UpdateItemAlbum(DTOUpdateAlbum model)
        {
            ArgumentNullException.ThrowIfNull(model);
            await updateValidator.ValidateAndThrowAsync(model);
            var answer = await repository.UpdateAsyncDb(Map(model));
            if (answer.updateModel is null)
            {
                logger.LogWarning("Operation update is album is fall!!!, messange {messege}",answer.Message);
                throw new UpdateItemBaseFail<AlbumService, DTOUpdateAlbum>("Update is failed");
            }
            try
            {
                await cache.IncrementAsync("albums:version");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
            }
            await hubContext.Clients.All.SendAsync("AlbumUpdated",model.id);
            return MapFull(answer.updateModel);
        }
        #endregion


        #region Remove 
        /// <summary>
        /// Remove one item album 
        /// </summary>
        /// <param name="id"> The ID of the album you want to delete </param>
        /// <returns>true is removed. This method can't return false</returns>
        /// <exception cref="RemoveItemBaseFail{AlbumService, Guid}"> if remove operaion is fall</exception>
        public async Task<bool> RemoveItemAlbum(Guid id)
        {
            string? result = await repository.RemoveAsyncDb(id);
            if (result is null) 
            {
                try {
                    await cache.IncrementAsync("albums:version");
                }
                catch (Exception ex) {
                    logger.LogCritical(ex.Message);
                }
                await hubContext.Clients.All.SendAsync("AlbumRemoved", id);
                return true;

            }
            
            logger.LogWarning("Operation remove is fall!! \n message: {result}",result);
            throw new RemoveItemBaseFail<AlbumService, Guid>(result);


        }
        #endregion


        #region Create

        /// <summary>
        /// Create one item album
        /// </summary>
        /// <param name="album">DTO create album</param>
        /// <returns>ID created item</returns>
        /// <exception cref="CreateItemBaseFail{AlbumService, DTOCreateAlbum}">if create operation is fall</exception>
        /// <exception cref="ValidationException">if validate is fall</exception>
        /// <exception cref="ArgumentNullException">if get DTOCreateAlbum is null</exception>


        public async Task<Guid> CreateItemAlbum(DTOCreateAlbum album)
        {
            ArgumentNullException.ThrowIfNull(album);
            await createValidator.ValidateAndThrowAsync(album);
            var result = await repository.CreateAsyncDb(Map(album));

            if (result.ID is null)
            {
                logger.LogWarning("Operation create is fall!! \n message: {message}",result.Message);
                throw new CreateItemBaseFail<AlbumService, DTOCreateAlbum>(result.Message);
            }
            try
            {
                await cache.IncrementAsync("albums:version");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex,"redis version not increment");
            }
            await hubContext.Clients.All.SendAsync("NewAlbumCreated", result.ID);
            return (Guid)result.ID;
        }

        #endregion


        #region Mapping 
        private RDTOTrack MapTrack(ModelTrackDB model) => mapper.Map<RDTOTrack>(model);

        private RDTOAlbumCard Map(ModelAlbumDB model) => mapper.Map<RDTOAlbumCard>(model);
        private ModelAlbumDB Map(DTOUpdateAlbum model) => mapper.Map<ModelAlbumDB>(model);
        private ModelAlbumDB Map(DTOCreateAlbum model) => mapper.Map<ModelAlbumDB>(model);

        private RDTOAlbum MapFull(ModelAlbumDB model) => mapper.Map<RDTOAlbum>(model);


        #endregion

    }
}
