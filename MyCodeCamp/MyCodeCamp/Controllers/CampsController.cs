using System;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        private readonly ICampRepository repository;

        public CampsController(ICampRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var camps = repository.GetAllCamps();

            return Ok(camps);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                camp = includeSpeakers ? repository.GetCampWithSpeakers(id) : repository.GetCamp(id);

                if (camp == null)
                    return NotFound($"Camp {id} was not found");

                return Ok(camp);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
