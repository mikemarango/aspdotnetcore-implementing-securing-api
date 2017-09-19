using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/speakers/{speakerId}/[controller]")]
    [ValidateModel]
    public class TalksController : BaseController
    {
        private readonly ILogger<TalksController> logger;
        private readonly IMapper mapper;
        private readonly ICampRepository repository;

        public TalksController(ICampRepository repository, ILogger<TalksController> logger, IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(string moniker, int speakerId)
        {
            var talks = repository.GetTalks(speakerId);

            if (talks.Any(t => t.Speaker.Camp.Moniker != moniker))
                return BadRequest("Invalid talks for the speaker selected");

            return Ok(mapper.Map<IEnumerable<TalkModel>>(talks));
        }

        [HttpGet("{id}", Name = "GetTalk")]
        public IActionResult Get(string moniker, int speakerId, int id)
        {
            var talk = repository.GetTalk(id);

            if (talk.Speaker.Id != speakerId || talk.Speaker.Camp.Moniker != moniker)
                return BadRequest("Invalid talk for the speaker selected");

            return Ok(mapper.Map<TalkModel>(talk));
        }

        [HttpPost()]
        public async Task<IActionResult> Post(string moniker, int speakerId, [FromBody] TalkModel model)
        {
            try
            {
                var speaker = repository.GetSpeaker(speakerId);

                if (speaker != null)
                {
                    var talk = mapper.Map<Talk>(model);

                    talk.Speaker = speaker;
                    repository.Add(talk);

                    if (await repository.SaveAllAsync())
                        return Created(Url.Link("GetTalk", new { moniker, speakerId, id = talk.Id }), mapper.Map<TalkModel>(talk));
                }

            }
            catch (Exception exception)
            {
                logger.LogError("Failed to save new talk", exception);
            }

            return BadRequest("Failed to save new talk");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string moniker, int speakerId, int id, [FromBody] TalkModel model)
        {
            try
            {
                var talk = repository.GetTalk(id);
                if (talk == null)
                    return NotFound();

                mapper.Map(model, talk);

                if (await repository.SaveAllAsync())
                    return Ok(mapper.Map<TalkModel>(talk));

            }
            catch (Exception exception)
            {
                logger.LogError($"Failed to update talk", exception);
            }

            return BadRequest("Failed to update talk");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string moniker, int speakerId, int id)
        {
            try
            {
                var talk = repository.GetTalk(id);

                if (talk == null)
                    return NotFound();

                repository.Delete(talk);

                if (await repository.SaveAllAsync())
                    return Ok();
            }
            catch (Exception exception)
            {
                logger.LogError("Failed to delete talk", exception);
            }

            return BadRequest("Failed to delete talk");
        }
    }
}
