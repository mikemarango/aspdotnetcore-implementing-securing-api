using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/[controller]")]
    [ValidateModel]
    public class SpeakersController : BaseController
    {
        private readonly ILogger<SpeakersController> logger;
        private readonly IMapper mapper;
        private readonly ICampRepository repository;
        private UserManager<CampUser> userManager;

        public SpeakersController(ILogger<SpeakersController> logger, IMapper mapper, ICampRepository repository, UserManager<CampUser> userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.repository = repository;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? repository.GetSpeakersByMonikerWithTalks(moniker) : repository.GetSpeakersByMoniker(moniker);

            return Ok(mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(string moniker, int id, bool includeTalks = false)
        {
            var speaker = includeTalks ? repository.GetSpeakerWithTalks(id) : repository.GetSpeaker(id);

            if (speaker == null)
                return NotFound();

            if (speaker.Camp.Moniker != moniker)
                return BadRequest("Speaker not in specified Camp");

            return Ok(mapper.Map<SpeakerModel>(speaker));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(string moniker, [FromBody]SpeakerModel model)
        {
            try
            {
                var camp = repository.GetCampByMoniker(moniker);
                if (camp == null)
                    return BadRequest("Could not find camp");

                var speaker = mapper.Map<Speaker>(model);
                speaker.Camp = camp;

                var campUser = await userManager.FindByNameAsync(User.Identity.Name);
                if (campUser != null)
                {
                    speaker.User = campUser;

                    repository.Add(speaker);

                    if (await repository.SaveAllAsync())
                    {
                        var url = Url.Link("SpeakerGet", new { moniker = camp.Moniker, id = speaker.Id });
                        return Created(url, mapper.Map<SpeakerModel>(speaker));
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while saving Speaker", exception);
            }

            return BadRequest("Could not add new speaker");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string moniker, int id, [FromBody] SpeakerModel model)
        {
            try
            {
                var speaker = repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Moniker != moniker)
                    return BadRequest("Speaker not in specified Camp");

                if (speaker.User.UserName != User.Identity.Name)
                    return Forbid();

                mapper.Map(model, speaker);

                if (await repository.SaveAllAsync())
                    return Ok(mapper.Map<SpeakerModel>(speaker));
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while updating a Speaker", exception);
            }

            return BadRequest("Could not update Speaker");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var speaker = repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Moniker != moniker)
                    return BadRequest("Speaker not in specified Camp");

                if (speaker.User.UserName != User.Identity.Name)
                    return Forbid();

                repository.Delete(speaker);

                if (await repository.SaveAllAsync())
                    return Ok();
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while deleting a Speaker", exception);
            }

            return BadRequest("Could not delete Speaker");
        }
    }
}
