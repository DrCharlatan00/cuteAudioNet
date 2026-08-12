using AutoMapper;
using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.Exceptions;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using cuteAudioNet.Services.Caching;

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


        // Add logger to service
        // and add doc to this 

        #region Get
        public async Task<IEnumerable<RDTOCardTrack>> GetTrackCardAsync()
        {
            const string cacheKey = "tracks:all_card";

            var cacheTracks = await cache.GetAsync<List<RDTOCardTrack>>(cacheKey);


            if (cacheTracks is not null) return cacheTracks;

            List<RDTOCardTrack> tracks = new();
            await foreach (var item in _tracksRepository.GetAllAsyncEnumerableDb())
            {
                tracks.Add(Map(item));
            }

            await cache.SetAsync(cacheKey,tracks,TimeSpan.FromMinutes(2));

            return tracks;
        }

        public async Task<IEnumerable<RDTOTrack>> GetAllTrackAsync()
        {
            const string cacheKey = "tracks:all";

            var cacheTracks = await cache.GetAsync<List<RDTOTrack>>(cacheKey);

            if (cacheTracks is not null) return cacheTracks;

            List<RDTOTrack> tracks = new();
            await foreach (var item in _tracksRepository.GetAllAsyncEnumerableDb())
            {
                tracks.Add(MapToFull(item));
            }

            await cache.SetAsync(cacheKey,tracks,TimeSpan.FromMinutes(2));
            return tracks;
        }


#warning Danger data return, review this method
        public async Task<ModelTrackDB?> GetWhisID(Guid id) {
            var data = await _tracksRepository.GetByIDAsyncDb(id);
            return data;
        }


        public async Task<IEnumerable<RDTOCardTrack>> GetByPaginationCard(int page, int pageSize) {

            const string cacheKey = "tracks:all_card";

            var cacheTracks = await cache.GetAsync<List<RDTOCardTrack>>(cacheKey);

            if (cacheTracks is not null) return cacheTracks.Skip((page - 1) * pageSize).Take(pageSize).ToImmutableList();

            var data = await _tracksRepository.GetWhisPaginationDb(page,pageSize);
            if (data is null) throw new DbGetCollectionIsNull("Collection is null",nameof(ModelTrackDB),nameof(GetByPaginationCard));
            return data.Select(Map).ToImmutableList();
        }
        #endregion

        #region Create

        public async Task<Guid> CreateAsync(DTOTrack newTrack)
        {
            ArgumentNullException.ThrowIfNull(newTrack);
            await _validator.ValidateAndThrowAsync(newTrack);
            var result = await _tracksRepository.CreateAsyncDb(Map(newTrack));
            if (result.ID is null) throw new CreateItemBaseFail<TrackService, DTOTrack>($"Can't create result, \nMessage {result.Message}");
            try
            {
                await cache.RemoveAsync("tracks:all_card");
                await cache.RemoveAsync("tracks:all");

            }
            catch (Exception ex) {
                logger.LogCritical($"data is redis not removed in class {nameof(TrackService)} \nException: {ex.Message}");
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
                    await cache.RemoveAsync("tracks:all_card");
                    await cache.RemoveAsync("tracks:all");

                }
                catch (Exception ex)
                {
                    logger.LogCritical($"data is redis not removed in class {nameof(TrackService)} \nException: {ex.Message}");
                }
                return true;
            }
            throw new RemoveItemBaseFail<TrackService, Guid>($"Track not remove whis error {res}");
        }
        #endregion

        #region Update
        public async Task<ModelTrackDB> Update(Guid id, DTOTrack dto)
        {
            var data = Map(dto);
            data.ID = id;
            var res = await _tracksRepository.UpdateTracksAsyncDb(data);
            if (res.UpdatedModel is null) throw new UpdateItemBaseFail<TrackService, DTOTrack>($"Update is have {res.Message} \n Data: {dto.ToString()}, ID: {id}");
            try
            {
                await cache.RemoveAsync("tracks:all_card");
                await cache.RemoveAsync("tracks:all");

            }
            catch (Exception ex)
            {
                logger.LogCritical($"data is redis not removed in class {nameof(TrackService)} \nException: {ex.Message}");
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
