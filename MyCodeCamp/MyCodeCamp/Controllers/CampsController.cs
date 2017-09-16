using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        private readonly ILogger<CampsController> logger;
        private readonly ICampRepository repository;

        public CampsController(ILogger<CampsController> logger, ICampRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var camps = repository.GetAllCamps();

            return Ok(camps);
        }

        [HttpGet("{id}", Name = "CampGet")]
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            try
            {
                var camp = includeSpeakers ? repository.GetCampWithSpeakers(id) : repository.GetCamp(id);

                if (camp == null)
                    return NotFound($"Camp {id} was not found");

                return Ok(camp);
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while getting a Camp", exception);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Camp model)
        {
            try
            {
                logger.LogInformation("Creating a new Code Camp");

                repository.Add(model);

                if (await repository.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { id = model.Id });
                    return Created(newUri, model);
                }

                logger.LogWarning("Could not save Camp to the database");
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while saving Camp", exception);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Camp model)
        {
            try
            {
                var oldCamp = repository.GetCamp(id);

                if (oldCamp == null)
                    return NotFound($"Camp {id} was not found");

                // Map model to the oldCamp
                oldCamp.Name = model.Name ?? oldCamp.Name;
                oldCamp.Description = model.Description?? oldCamp.Description;
                oldCamp.Location = model.Location ?? oldCamp.Location;
                oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;
                oldCamp.EventDate = model.EventDate != DateTime.MinValue ? model.EventDate : oldCamp.EventDate;

                if (await repository.SaveAllAsync())
                    return Ok(oldCamp);
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while updating a Camp", exception);
            }

            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldCamp = repository.GetCamp(id);

                if (oldCamp == null)
                    return NotFound($"Camp {id} was not found");

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
