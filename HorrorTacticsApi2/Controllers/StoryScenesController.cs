﻿using HorrorTacticsApi2.Domain;
using HorrorTacticsApi2.Domain.Dtos;
using HorrorTacticsApi2.Domain.Models.Stories;
using HorrorTacticsApi2.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorrorTacticsApi2.Controllers
{
    [Authorize]
    [Route($"{Constants.SecuredApiPath}")]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ScenesController : HtController
    {
        readonly StoryScenesService _service;
        
        public ScenesController(StoryScenesService service)
        { 
            _service = service;
        }

        [HttpGet("stories/[controller]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadStorySceneModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var model = await _service.TryGetAsync(GetUser(), id, true, token);
            if (model == default)
                return NotFound();

            return Ok(model);
        }

        [HttpGet("stories/{storyId}/[controller]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadStorySceneModel>> GetAll([FromRoute] long storyId, CancellationToken token)
        {
            var model = await _service.GetAllAsync(GetUser(), storyId, true, token);
            
            return Ok(model);
        }

        [HttpPost("stories/{storyId}/[controller]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadStorySceneModel>> Post([FromRoute] long storyId, [FromBody] CreateStorySceneModel model, CancellationToken token)
        {
            var readModel = await _service.CreateStorySceneAsync(GetUser(), storyId, model, true, token);
            return CreatedAtAction(nameof(Get), new { id = readModel.Id }, readModel);
        }

        [HttpPut("stories/[controller]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadStorySceneModel>> Put([FromRoute] long id, [FromBody] UpdateStorySceneModel model, CancellationToken token)
        {
            return Ok(await _service.UpdateStorySceneAsync(GetUser(), id, model, true, token));
        }

        [HttpDelete("stories/[controller]/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            await _service.DeleteStorySceneAsync(GetUser(), id, token);
            return NoContent();
        }

    }
}
