﻿using HorrorTacticsApi2.Domain;
using HorrorTacticsApi2.Domain.Dtos;
using HorrorTacticsApi2.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorrorTacticsApi2.Controllers
{
    [Authorize]
    [Route($"{Constants.SecuredApiPath}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ImagesController : HtController
    {
        readonly ImagesService _service;
        
        public ImagesController(ImagesService service)
        { 
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<ReadImageModel>>> Get(CancellationToken token)
        {
            return Ok(await _service.GetAllImagesAsync(GetUser(), token));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadImageModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var model = await _service.TryGetAsync(GetUser(), id, token);
            if (model == default)
                return NotFound();

            return Ok(model);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(Constants.MULTIPART_FORMDATA), Produces(MediaTypeNames.Application.Json)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<ReadImageModel>> Post(CancellationToken token)
        {
            // Multi part request is handled by the service (with IHttpContextAccessor)
            var readModel = await _service.CreateImageAsync(GetUser(), token);
            return CreatedAtAction(nameof(Get), new { id = readModel.Id }, readModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReadImageModel>> Put([FromRoute] long id, [FromBody] UpdateImageModel model, CancellationToken token)
        {
            return Ok(await _service.UpdateImageAsync(GetUser(), id, model, true, token));
        }

        [HttpPut("{id}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(Constants.MULTIPART_FORMDATA), Produces(MediaTypeNames.Application.Json)]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<ReadImageModel>> PutFile([FromRoute] long id, CancellationToken token)
        {
            return Ok(await _service.ReplaceImageFileAsync(GetUser(), id, token));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            await _service.DeleteImageAsync(GetUser(), id, token);
            return NoContent();
        }
    }
}
