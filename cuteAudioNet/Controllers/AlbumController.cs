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
        [HttpGet]
        public async Task<IActionResult> GetCardAsync() 
        {
            var data = await service.GetAllFromCardAsync();
            return data is not null ? Ok(data) : BadRequest();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullAsync() 
        {
            var data = await service.GetFullInfomaionAlbumAsync();
            return data is not null ? Ok(data) : BadRequest();
        }

        [HttpGet("pag")]
        public async Task<IActionResult> GetWhisPaginationCardAsync([FromQuery] int page, [FromQuery] int pageSize) {
            var data = await service.GetByPaginationCard(page, pageSize);
            return data is not null ? Ok(data) : BadRequest();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIDAsync(Guid id) {
            var item = await service.GetByIDAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> UpdateAlbumAsync([FromBody] DTOUpdateAlbum album) {
            RDTOAlbum item = await service.UpdateItemAlbum(album);
            return item is not null ? Ok(item) : BadRequest(); 
        }
        #endregion

        #region Create
        [HttpPost]
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
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAlbumAsync(Guid id) {
            var result = await service.RemoveItemAlbum(id);
            return result ? Ok(new {
                Message = "Remove success",
                ID = result
            }) 
            : BadRequest();
        }
        #endregion


    }
}
