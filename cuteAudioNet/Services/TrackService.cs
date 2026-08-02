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

namespace cuteAudioNet.Services
{
    public class TrackService(
        ITracksRepository tracksRepository,
        IMapper mapper,
        IValidator<DTOTrack> validator) : ITrackService
    {
        private readonly ITracksRepository _tracksRepository = tracksRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<DTOTrack> _validator = validator;


        #region Get
        public async Task<IEnumerable<RDTOCardTrack>> GetTrackCardAsync()
        {
            List<RDTOCardTrack> tracks = new();
            await foreach (var item in _tracksRepository.GetAllAsyncEnumerableDb())
            {
                tracks.Add(Map(item));
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

        public async Task<ModelTrackDB?> GetWhisID(Guid id) {
            var data = await _tracksRepository.GetByIDAsyncDb(id);
            return data;
        }


        public async Task<IEnumerable<RDTOCardTrack>> GetByPaginationCard(int page, int pageSize) {
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
            return (Guid)result.ID;
        }
        #endregion

        #region Remove
        public async Task<bool> RemoveAsync(Guid id)
        {
            string? res = await _tracksRepository.RemoveAsyncDb(id);
            if (res is null) return true;
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
