using AutoMapper;
using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Exceptions;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using FluentValidation;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace cuteAudioNet.Services
{
    public class AlbumService(
     /// TODO : 
     /// Create Validation Service 
     /// Create Test Critical code
     ///

        IAlbumsRepository repository,
        ILogger<AlbumService> logger,
        IMapper mapper
        )
    {
        private readonly IAlbumsRepository repository = repository;
        private readonly ILogger<AlbumService> logger = logger;
        private readonly IMapper mapper = mapper;

        #region Get 
        /// <summary>
        /// Get Albums whis Card 
        /// </summary>
        /// <returns>Collections Albums Card</returns>
        /// <remarks>This not tested code,very possible his not work </remarks>

#warning Untested

        public async Task<IEnumerable<RDTOAlbumCard>> GetAllFromCardAsync() {
            List<RDTOAlbumCard> cards = new();
            await foreach ((string AlbumName, string ArtistNickname) data in repository.GetAsyncEnumerebleFromCardDb())
            {
                cards.Add(new RDTOAlbumCard (data.AlbumName,data.ArtistNickname));
            }
            return cards;
        }

        /// <summary>
        ///  Get Albums whis artist and track
        /// </summary>
        /// <returns> collection is Albums whis tracks</returns>
        /// <exception cref="DbGetCollectionIsNull"></exception>
        /// <remarks>This not tested code,very possible his not work </remarks>

        [Experimental("NOT_REQUIRED_TESTED_METHOD")]
        public async Task<IEnumerable<RDTOAlbum>> GetFullInfomaionAlbumAsync()
        {
            List<RDTOAlbum> data = new List<RDTOAlbum>();
            await foreach (var item in repository.GetAsyncEnumerableAllAlbumDb())
            {
                if (item is null) throw new DbGetCollectionIsNull("In await foreach" , nameof(ModelAlbumDB),nameof(RDTOAlbum));
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
            await repository.GetOnlyAlbums();
            return data;

        }
        /// <summary>
        /// Get album by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>RDTO model Album whis track and artist</returns>
#warning Untested GetByIDAsync
        public async Task<RDTOAlbum?> GetByIDAsync(Guid id) {
            ModelAlbumDB item = await repository.GetByIdAsyncDb(id);
            if (item is null) return null;
            return MapFull(item);
        }


        /// <summary>
        /// Get by pagination card 
        /// </summary>
        /// <param name="page">page in site</param>
        /// <param name="pageSize">count elements for site</param>
        /// <returns>Collection RDTOAlbumCard </returns>
        /// <exception cref="DbGetCollectionIsNull">  possible if db return null</exception>
#warning Untested GetByPaginationCard
        public async Task<IEnumerable<RDTOAlbumCard>> GetByPaginationCard(int page, int pageSize) {
            var data = await repository.GetWhisPaginationAsyncDb(page, pageSize);
            if (data is null) throw new DbGetCollectionIsNull(null,nameof(ModelAlbumDB),nameof(GetByPaginationCard));
            return data.Select(Map).ToImmutableList();
        }


        #endregion
        [Experimental("NOT_REQUIRED_TESTED_METHOD")]
        #region Update
        public async Task<RDTOAlbum> UpdateAlbum(DTOUpdateAlbum model) {
            ArgumentNullException.ThrowIfNull(model);
            var answer = await repository.UpdateAsyncDb(Map(model));
            if (answer.updateModel is null) {
                logger.LogWarning($"Operation update is album is fall!!!, messange {answer.Message}");
                throw new UpdateItemBaseFail<AlbumService, DTOUpdateAlbum>("Update is failed");
            }
            return MapFull(answer.updateModel);
        }
        #endregion


        #region Mapping 
        private RDTOTrack MapTrack(ModelTrackDB model) => mapper.Map<RDTOTrack>(model);

        private RDTOAlbumCard Map(ModelAlbumDB model) => mapper.Map<RDTOAlbumCard>(model);
        private ModelAlbumDB Map(DTOUpdateAlbum model) => mapper.Map<ModelAlbumDB>(model);
        private RDTOAlbum MapFull(ModelAlbumDB model) => mapper.Map<RDTOAlbum>(model);


        #endregion

    }
}
