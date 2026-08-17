using cuteAudioNet.APIModels.DTO.Artists;
using cuteAudioNet.APIModels.RDTOModel.Artists;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services;
using cuteAudioNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace cuteAudioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController(IArtistService service) : ControllerBase
    {
        private readonly IArtistService service = service;

        #region Get

        /// <summary>
        /// Get card collection artists
        /// </summary>
        /// <returns>card collection artists</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(IEnumerable<RDTOArtistCard>))]
        [ProducesResponseType (StatusCodes.Status500InternalServerError, Description = "Return message")]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        /// <summary>
        /// Get full info Artists collection
        /// </summary
        /// <returns>full info Artists collection</returns>
        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOArtist>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Return message")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken) {
            var data = await service.GetFullArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        /// <summary>
        /// Get collection only have info artist
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns> collection only have info artist</returns>
        /// <remarks>This collection have only information artist</remarks>
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOOnlyArtistInfo>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Return message")]
        public async Task<IActionResult> GetInformationArtists(CancellationToken cancellationToken) {
            var data = await service.GetInfoArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        /// <summary>
        /// Get artist with id
        /// </summary>
        /// <param name="id">ID Artist</param>
        /// <returns>Full Artist RDTO</returns>

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOOnlyArtistInfo>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(Guid id) {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Get card collection on pagination
        /// </summary>
        /// <param name="page">Number page</param>
        /// <param name="pageSize">count items</param>
        /// <returns>card collection tracks with pagination</returns>

        [HttpGet("pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOArtistCard>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Return message")]
        public async Task<IActionResult> GetPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await service.GetCardWithPagination(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        /// <summary>
        /// Search by nickname artist
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Collection sorted by name</returns>
        /// <remarks>Search By NickName, Don't forgot this</remarks>
        [HttpGet("by-name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOArtist>))]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await service.SearchByNIckNameAsync(name, cancellationToken);
            return Ok(data);
        }


        /// <summary>
        /// Search by nickname artist with pagination
        /// </summary>
        /// <param name="name">the nick by which you want to search</param>
        /// <param name="page">Number page</param>
        /// <param name="pageSize">count items</param>
        /// <returns>Collection artist sorted by name with pagination</returns>
        [HttpGet("by-name-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOArtistCard>))]
        public async Task<IActionResult> SearchByNamePagination(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNickNameWithPaginationAsync(name, page, pageSize, cancellationToken);
            return Ok(data);

        }
        #endregion

        #region Update

        /// <summary>
        /// Update artist
        /// </summary>
        /// <param name="id">ID Update artist</param>
        /// <param name="model">New Information for Artist</param>
        /// <returns>RDTO model new Artist</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOArtist>))]
        public async Task<IActionResult> Update(Guid id,[FromBody] DTOArtist model) {
            var item = await service.UpdateAsync(id, model);
            return Ok(item);
        }
        #endregion


        #region Create
        /// <summary>
        /// Create new artist
        /// </summary>
        /// <param name="model">New artist</param>
        /// <returns>ID Artist</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]

        public async Task<IActionResult> Create(DTOArtist model) {
            var result = await service.CreateAsync(model);
            return Ok(result);
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove artist by id
        /// </summary>
        /// <param name="id">ID Artist </param>
        /// <returns>Ok or BadRequest</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(Guid id) {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok() : BadRequest();
        }
        #endregion

    }
}
