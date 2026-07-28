using cuteAudioNet.APIModels.DTO;
using cuteAudioNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace cuteAudioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController(ITrackService trackService) : ControllerBase
    {
        private readonly ITrackService _trackService = trackService;

        #region Get
        [HttpGet]
        public async Task<IActionResult> GetCardTrackAsync() {
            var data = await _trackService.GetTrackCardAsync();
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetTracksFullInfomationAsync() {
            var data = await _trackService.GetAllTrackAsync();
            if (data is null)  return NotFound();
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTrackWhisID(Guid id) {
            var data = await _trackService.GetWhisID(id);
            return data is null ? NotFound() : Ok(data);
        }

        [HttpGet("pag-card")]
        public async Task<IActionResult> GetCardTrackWhisPag() {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTrackAsync(Guid id, DTOTrack Track) {
            var result = await _trackService.Update(id,Track);
            if (result is null) return BadRequest(new {
                Message = "Update is failed ",
                Operation = "Update Track"
            });
            return Ok(new {
                Model = result,
            });
        }
        #endregion

        #region Create
        [HttpPost("create")]
        public async Task<IActionResult> CreateTrackAsync(DTOTrack newTrack) {
            var res = await _trackService.CreateAsync(newTrack);
            return CreatedAtAction(
                    actionName: nameof(GetTrackWhisID),
                    routeValues: new { id = res },
                    value: res
                );
        }
        #endregion

        #region Delete
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id) {
            return await _trackService.RemoveAsync(id) ? Ok() : BadRequest();
        }
        #endregion

    }
}
