using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cuteAudioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController(IAlbumService service) : ControllerBase
    {
        private readonly IAlbumService service = service;

        #region Get
        ///  <summary>
        /// Get Collection In Card Album 
        /// </summary>
        ///<returns>
        /// Ok and collection data
        /// -Name
        /// -ArtistName
        /// , if collection is null or bad return BadRequest
        /// </returns>
        /// <remarks>
        /// Don't forgot this return only card infomation
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOAlbumCard>))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCardAsync() 
        {
            var data = await service.GetAllFromCardAsync();
            return data is not null ? Ok(data) : BadRequest();
        }
		
    /// <summary>
    /// Method return full information on Album
    /// </summary>
    /// <returns>
    /// return Ok and collection data
    /// -string Name,string?
    /// -DateRelease,
    /// -string ArtistName,
    /// -Collection Tracks
    /// or if collection is null or bad return BadRequest
    /// </returns>
    [HttpGet("full")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOAlbum>))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFullAsync() 
        {
            var data = await service.GetFullInfomaionAlbumAsync();
            return data is not null ? Ok(data) : BadRequest();
        }
/// <summary>
/// Method return card for web page 
/// </summary>
/// <param name="page">current page number</param>
/// <param name="pageSize">count item for page</param>
/// <returns> collection card Album form page setting </returns>
/// <remarks>Do not forgot, return card collection</remarks>
        [HttpGet("pag")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOAlbumCard>))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWhisPaginationCardAsync([FromQuery] int page, [FromQuery] int pageSize) {
            var data = await service.GetByPaginationCard(page, pageSize);
            return data is not null ? Ok(data) : BadRequest();
        }
/// <summary>
/// Get Album by id
/// </summary>
/// <param name="id">ID Album  in db</param>
/// <returns>
///  Album with vars :
/// -string Name,string?
/// -DateRelease,
/// -string ArtistName,
/// -Collection Tracks
/// </returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(RDTOAlbum))]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIDAsync(Guid id) {
            var item = await service.GetByIDAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        #endregion

        #region Update
        /// <summary>
        /// Update information album in db
        /// </summary>
        /// <param name="album">Method wait this info
        ///Guid id,
        ///string AlbumName,
        ///string DateRelease or null
        /// </param>
        /// <returns>
        /// If operation is ok return Ok and updated item
        /// Else return BadRequest
        /// </returns>
        /// <remarks>Method not update Guid</remarks>

        [HttpPut]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(RDTOAlbum))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAlbumAsync([FromBody] DTOUpdateAlbum album) {
            RDTOAlbum item = await service.UpdateItemAlbum(album);
            return item is not null ? Ok(item) : BadRequest(); 
        }
        #endregion

        #region Create
        /// <summary>
        /// Create new Album element in db 
        /// </summary>
        /// <param name="album">Method wait next infomation in body
        ///string Name, 
        ///string DateRelease or null,
        /// Guid IdArtist
        /// </param>
        /// <returns>if ok return ok and
        ///      id = item, id item create
        ///      Where = "Get + guid" where get item
        ///</returns>
        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> CreateAlbumAsync([FromBody] DTOCreateAlbum album) {
            Guid item = await service.CreateItemAlbum(album);
            //return CreatedAtAction(nameof(GetByIDAsync), new {id = item }, item);  I don't no why he not work
            return Ok(new {
                id = item,
                Where = "Get + guid"
            });
        }
        #endregion


        #region Delete
        /// <summary>
        /// Remove Album item in DB for ID Key
        /// </summary>
        /// <param name="id">ID Key for Query</param>
        /// <returns>
        ///{
        ///Message - Result Operation run
        /// }
        /// </returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAlbumAsync(Guid id) {
            var result = await service.RemoveItemAlbum(id);
            return result ? Ok(new {
                Message = "Remove success",
            }) 
            : BadRequest();
        }
        #endregion


    }
}
