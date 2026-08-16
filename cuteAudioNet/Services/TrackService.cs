using AutoMapper;
using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Exceptions;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services.Caching;
using cuteAudioNet.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace cuteAudioNet.Services
{
    public class TrackService(
        ITracksRepository tracksRepository,
        IMapper mapper,
        IValidator<DTOTrack> validator,
        ICacheService cache,
        ILogger<TrackService> logger
        ) : ITrackService
    {
        private readonly ITracksRepository _tracksRepository = tracksRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<DTOTrack> _validator = validator;
        private readonly ICacheService cache = cache;

        #region Get
        
        public async Task<IEnumerable<RDTOCardTrack>> GetTrackCardAsync()
        {

            List<RDTOCardTrack> tracks = new();
            await foreach ((string Name, Postgresql.Models.MusicGenre Genre, string ArtistNickname) item in _tracksRepository.GetAllTrackCardAsyncEnumerableDb())
            {
                tracks.Add(new RDTOCardTrack(item.Name,(APIModels.RDTOModel.Tracks.MusicGenre)item.Genre,item.ArtistNickname));
            }

            return tracks;
        }

        public async Task<IEnumerable<RDTOTrack>> GetAllTrackAsync()
        {


        
            List<RDTOTrack> tracks = new();
            await foreach (var item in _tracksRepository.GetAllAsyncEnumerableDb())
            {
                tracks.Add(MapToFull(item));
            }

            return tracks;
        }



        public async Task<RDTOTrack?> GetByIDAsync(Guid id) {
            var data = await _tracksRepository.GetByIDAsyncDb(id);
            return data is not  null ? MapToFull(data) : null;
        }


        public async Task<IEnumerable<RDTOCardTrack>> GetByPaginationCardAsync(int page, int pageSize) {


            if (page <= 0 || page > 10000)
            {
                throw new ArgumentException("Page is bad");
            }

            if (pageSize <= 0 || pageSize > 10000)
            {
                throw new ArgumentException("Page size is bad");

            }

            const string versionCache = "tracks:version";
            var version = await cache.GetVersionAsync(versionCache);
            string cacheKey = $"tracks:card:v{version}:page{page}:size:{pageSize}";


            try
            {
                var cacheTracks = await cache.GetAsync<List<RDTOCardTrack>>(cacheKey);
                if (cacheTracks is not null) return cacheTracks;
            }
            catch (Exception ex) {
                logger.LogError(ex.Message);
            }

            var data = await _tracksRepository.GetWhisPaginationDb(page,pageSize);
            if (data is null) {
                logger.LogInformation("Get null in collection whis pagination method");
                throw new DbGetCollectionIsNull("Collection is null", nameof(ModelTrackDB), nameof(GetByPaginationCardAsync));
            }

            try
            {
                List <RDTOCardTrack> datac = data.Select(Map).ToList();
                await cache.SetAsync<List<RDTOCardTrack>>(cacheKey, datac, TimeSpan.FromMinutes(2));
            }
            catch (Exception ex) {
                logger.LogError(ex,"redis data not saved");
            }
            return data.Select(Map).ToImmutableList();
        }

        public async Task<IEnumerable<RDTOTrack>> SearchByNameAsync(string name, CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is bad or null");

           
            List<RDTOTrack> tracks = new();
            await foreach (var item in tracksRepository.SearchByNameAsyncEnumerable(name, cancellationToken)) {
                tracks.Add(MapToFull(item));
            }
            return tracks;
        }

        public async Task<IEnumerable<RDTOCardTrack>> SearchByNamePaginationAsync(string name,int page, int pageSize ,CancellationToken cancellationToken) {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is bad or null");


            if (page <= 0 || page > 10000)
            {
                throw new ArgumentException("Page is bad");
            }

            if (pageSize <= 0 || pageSize > 10000)
            {
                throw new ArgumentException("Page size is bad");
            }

            List<RDTOCardTrack> tracks = new();
            await foreach (var item in tracksRepository.SearchByNameWithPaginationAsyncEnumerable(name,page, pageSize, cancellationToken))
            {
                tracks.Add(Map(item));
            }
            return tracks;
        }
        #endregion

        #region Create

        public async Task<Guid> CreateAsync(DTOTrack newTrack)
        {
            ArgumentNullException.ThrowIfNull(newTrack);
            await _validator.ValidateAndThrowAsync(newTrack);
            var result = await _tracksRepository.CreateAsyncDb(Map(newTrack));
            if (result.ID is null) {
                logger.LogInformation("Item not created, Model : {@model},\nMessage: {message}",newTrack,result.Message);
                throw new CreateItemBaseFail<TrackService, DTOTrack>($"Can't create item, \nMessage {result.Message}"); 
            }
            try
            {
                await cache.IncrementAsync("tracks:version");

            }
            catch (Exception ex) {
                //logger.LogCritical($"data is redis not removed in class {nameof(TrackService)} \nException: {ex.Message}");
                logger.LogCritical("data is redis not removed with create in class {track} \nException {exc}", nameof(TrackService), ex.Message);
            }
            return (Guid)result.ID;
        }
        #endregion

        #region Remove
        public async Task<bool> RemoveAsync(Guid id)
        {
            string? res = await _tracksRepository.RemoveAsyncDb(id);
            if (res is null)
            {
                try
                {
                    await cache.IncrementAsync("tracks:version");
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"data is redis not removed in class {nameof(TrackService)} \nException: {ex.Message}");
                }
                return true;
            }
            logger.LogInformation("Not remove item with id: {id}, Message: {res}",id,res);
            throw new RemoveItemBaseFail<TrackService, Guid>($"Track not remove whis error {res}");
        }
        #endregion

        #region Update
        public async Task<ModelTrackDB> UpdateAsync(Guid id, DTOTrack dto)
        {
            var data = Map(dto);
            data.ID = id;
            var res = await _tracksRepository.UpdateTracksAsyncDb(data);
            if (res.UpdatedModel is null) {
                logger.LogInformation("Item not Update, info:{id},{dto}\n Message: {res}",id,dto.ToString(),res.Message);
                throw new UpdateItemBaseFail<TrackService, DTOTrack>($"Update is have {res.Message} \n Data: {dto.ToString()}, ID: {id}"); 
            }
            try
            {
                await cache.IncrementAsync("tracks:version");
            }
            catch (Exception ex)
            {
                logger.LogCritical("data is redis not removed with update in class {track} \nException {exc}", nameof(TrackService), ex.Message);
            }
            return res.UpdatedModel;
        }
        #endregion

        #region Map
        private RDTOCardTrack Map(ModelTrackDB model) => _mapper.Map<RDTOCardTrack>(model);
        private RDTOTrack MapToFull(ModelTrackDB model) => _mapper.Map<RDTOTrack>(model);
        private ModelTrackDB Map(DTOTrack track) => _mapper.Map<ModelTrackDB>(track);
        #endregion
    }
}
