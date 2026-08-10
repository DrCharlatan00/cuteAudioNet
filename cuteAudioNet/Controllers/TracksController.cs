using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Models;
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
        /// <summary>
        /// Get Track card info
        /// </summary>
        /// <returns>Collection Card information Track</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCardTrack>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCardTrackAsync() {
            IEnumerable<RDTOCardTrack>? data = await _trackService.GetTrackCardAsync();
            if (data is null) return NotFound();
            return Ok(data);
        }

        /// <summary>
        /// Get full information track
        /// </summary>
        /// <returns>Collection full information on tracks</returns>
        /// <remarks>Don't use this endpoint if you only need the name, genre, artist</remarks>

        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOTrack>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTracksFullInfomationAsync() {
            IEnumerable<RDTOTrack>? data = await _trackService.GetAllTrackAsync();
            if (data is null)  return NotFound();
            return Ok(data);
        }

        /// <summary>
        /// Get full info with id track
        /// </summary>
        /// <param name="id">GUID Track</param>
        /// <returns>Full infomation on track</returns>

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(RDTOTrack))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrackWhisID(Guid id) {
            var data = await _trackService.GetWhisID(id);
            return data is null ? NotFound() : Ok(data);
        }

        /// <summary>
        /// Do Not use, Method not released 
        /// </summary>
        /// <returns>Dead</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>This not work, do not use method</remarks>
        [Experimental("NOT_RELEASED_CODE")]
        [HttpGet("pag-card")]
        public async Task<IActionResult> GetCardTrackWhisPag() {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        /// <summary>
        /// Update track in db 
        /// </summary>
        /// <param name="id">ID Track in db</param>
        /// <param name="Track">
        /// Infotmation on track
        /// string Name,
        /// Guid AlbumID,
        /// MusicGenre Genre,
        /// string TimeRelease or null,
        /// string SubArtist or null
        /// </param>
        /// <returns>Updated model</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ModelTrackDB))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTrackAsync(Guid id,[FromBody] DTOTrack Track) {
            ModelTrackDB? result = await _trackService.Update(id,Track); // review, maybe return in RDTO 
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
        /// <summary>
        /// Create new track in db
        /// </summary>
        /// <param name="newTrack">
        /// string Name, Guid AlbumID,MusicGenre Genre, string TimeRelease or null, string SubArtist or null
        /// </param>
        /// <returns>Create At Action and id new track</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <summary>
        /// Remove Track in db with id
        /// </summary>
        /// <param name="id">GUID Track</param>
        /// <returns>OK or BadRequest if false</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(Guid id) {
            return await _trackService.RemoveAsync(id) ? Ok() : BadRequest();
        }
        #endregion

    }
}
