using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using System.Threading.Tasks;
using AutoMapper;
using MyCodeCamp.Models;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : BaseController
    {
        private readonly ILogger<CampsController> logger;
        private readonly IMapper mapper;
        private readonly ICampRepository repository;

        public CampsController(ILogger<CampsController> logger, IMapper mapper, ICampRepository repository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var camps = repository.GetAllCamps();

            return Ok(mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{moniker}", Name = "CampGet")]
        public IActionResult Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var camp = includeSpeakers ? repository.GetCampByMonikerWithSpeakers(moniker) : repository.GetCampByMoniker(moniker);

                if (camp == null)
                    return NotFound($"Camp {moniker} was not found");

                return Ok(mapper.Map<CampModel>(camp));
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while getting a Camp", exception);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CampModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                logger.LogInformation("Creating a new Code Camp");

                var camp = mapper.Map<Camp>(model);

                repository.Add(camp);

                if (await repository.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { moniker = camp.Moniker });
                    return Created(newUri, mapper.Map<CampModel>(camp));
                }

                logger.LogWarning("Could not save Camp to the database");
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while saving Camp", exception);
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker, [FromBody] CampModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var oldCamp = repository.GetCampByMoniker(moniker);

                if (oldCamp == null)
                    return NotFound($"Camp {moniker} was not found");
                
                mapper.Map(model, oldCamp);

                if (await repository.SaveAllAsync())
                    return Ok(mapper.Map<CampModel>(oldCamp));
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while updating a Camp", exception);
            }

            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = repository.GetCampByMoniker(moniker);

                if (oldCamp == null)
                    return NotFound($"Camp {moniker} was not found");

                repository.Delete(oldCamp);

                if (await repository.SaveAllAsync())
                    return Ok();
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while deleting a Camp", exception);
            }

            return BadRequest("Couldn't delete Camp");
        }
    }
}
