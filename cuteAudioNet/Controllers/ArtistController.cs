using cuteAudioNet.APIModels.DTO.Artists;
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
        [HttpGet]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken) {
            var data = await service.GetFullArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }
        [HttpGet("info")]
        public async Task<IActionResult> GetInformationArtists(CancellationToken cancellationToken) {
            var data = await service.GetInfoArtistAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id) {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpGet("pag")]
        public async Task<IActionResult> GetPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await service.GetCardWithPagination(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection now not avaible");
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await service.SearchByNIckNameAsync(name, cancellationToken);
            return Ok(data);
        }
        [HttpGet("by-name-pag")]
        public async Task<IActionResult> SearchByNamePagination(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNickNameWithPaginationAsync(name, page, pageSize, cancellationToken);
            return Ok(data);

        }
        #endregion

        #region Update
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] DTOArtist model) {
            var item = await service.UpdateAsync(id, model);
            return Ok(item);
        }
        #endregion


        #region Create
        [HttpPost]
        public async Task<IActionResult> Create(DTOArtist model) {
            var result = await service.CreateAsync(model);
            return Ok(model);
        }
        #endregion

        #region Remove
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove(Guid id) {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : BadRequest();
        }
        #endregion

    }
}
