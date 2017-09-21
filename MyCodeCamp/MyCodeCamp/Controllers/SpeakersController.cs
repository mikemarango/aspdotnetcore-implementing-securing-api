using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Route("api/camps/{moniker}/[controller]")]
    [ValidateModel]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class SpeakersController : BaseController
    {
        protected readonly ILogger<SpeakersController> Logger;
        protected readonly IMapper Mapper;
        protected readonly ICampRepository Repository;
        protected readonly UserManager<CampUser> UserManager;

        public SpeakersController(ILogger<SpeakersController> logger, IMapper mapper, ICampRepository repository, UserManager<CampUser> userManager)
        {
            Logger = logger;
            Mapper = mapper;
            Repository = repository;
            UserManager = userManager;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult Get(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? Repository.GetSpeakersByMonikerWithTalks(moniker) : Repository.GetSpeakersByMoniker(moniker);

            return Ok(Mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet]
        [MapToApiVersion("1.1")]
        public virtual IActionResult GetWithCount(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? Repository.GetSpeakersByMonikerWithTalks(moniker) : Repository.GetSpeakersByMoniker(moniker);

            return Ok(new{ count = speakers.Count(), results = Mapper.Map<IEnumerable<SpeakerModel>>(speakers) });
        }

        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(string moniker, int id, bool includeTalks = false)
        {
            var speaker = includeTalks ? Repository.GetSpeakerWithTalks(id) : Repository.GetSpeaker(id);

            if (speaker == null)
                return NotFound();

            if (speaker.Camp.Moniker != moniker)
                return BadRequest("Speaker not in specified Camp");

            return Ok(Mapper.Map<SpeakerModel>(speaker));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(string moniker, [FromBody]SpeakerModel model)
        {
            try
            {
                var camp = Repository.GetCampByMoniker(moniker);
                if (camp == null)
                    return BadRequest("Could not find camp");

                var speaker = Mapper.Map<Speaker>(model);
                speaker.Camp = camp;

                var campUser = await UserManager.FindByNameAsync(User.Identity.Name);
                if (campUser != null)
                {
                    speaker.User = campUser;

                    Repository.Add(speaker);

                    if (await Repository.SaveAllAsync())
                    {
                        var url = Url.Link("SpeakerGet", new { moniker = camp.Moniker, id = speaker.Id });
                        return Created(url, Mapper.Map<SpeakerModel>(speaker));
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("Threw exception while saving Speaker", exception);
            }

            return BadRequest("Could not add new speaker");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string moniker, int id, [FromBody] SpeakerModel model)
        {
            try
            {
                var speaker = Repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Moniker != moniker)
                    return BadRequest("Speaker not in specified Camp");

                if (speaker.User.UserName != User.Identity.Name)
                    return Forbid();

                Mapper.Map(model, speaker);

                if (await Repository.SaveAllAsync())
                    return Ok(Mapper.Map<SpeakerModel>(speaker));
            }
            catch (Exception exception)
            {
                Logger.LogError("Threw exception while updating a Speaker", exception);
            }

            return BadRequest("Could not update Speaker");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var speaker = Repository.GetSpeaker(id);

                if (speaker == null)
                    return NotFound();

                if (speaker.Camp.Moniker != moniker)
                    return BadRequest("Speaker not in specified Camp");

                if (speaker.User.UserName != User.Identity.Name)
                    return Forbid();

                Repository.Delete(speaker);

                if (await Repository.SaveAllAsync())
                    return Ok();
            }
            catch (Exception exception)
            {
                Logger.LogError("Threw exception while deleting a Speaker", exception);
            }

            return BadRequest("Could not delete Speaker");
        }
    }
}
