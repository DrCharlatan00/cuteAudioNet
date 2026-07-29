using AutoMapper;
using cuteAudioNet.APIModels.Exceptions;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
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
        public async Task<IEnumerable<RDTOAlbumCard>> GetAllFromCardAsync() {
            List<RDTOAlbumCard> cards = new();
            await foreach (ModelAlbumDB ModelAlbumDB in repository.GetAsyncEnumerableAllAlbumDb())
            {
                cards.Add(Map(ModelAlbumDB));
            }
            return cards;
        }

        /// <summary>
        ///  Get Albums whis artist and track
        /// </summary>
        /// <returns> collection is Albums whis tracks</returns>
        /// <exception cref="DbGetCollectionIsNull"></exception>
        /// <remarks>This not tested code,very possible his not work </remarks>

        [Experimental("NOT_REQUIRED_TESTED_SERVICE")]
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


        #endregion


        #region Mapping 
        private RDTOTrack MapTrack(ModelTrackDB model) => mapper.Map<RDTOTrack>(model);

        private RDTOAlbumCard Map(ModelAlbumDB model) => mapper.Map<RDTOAlbumCard>(model);
        private RDTOAlbum MapFull(ModelAlbumDB model) => mapper.Map<RDTOAlbum>(model);

        #endregion

    }
}
