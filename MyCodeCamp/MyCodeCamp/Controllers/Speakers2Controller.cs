using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/Speakers")]
    [ApiVersion("2.0")]
    public class Speakers2Controller : SpeakersController
    {
        public Speakers2Controller(ILogger<SpeakersController> logger, IMapper mapper, ICampRepository repository, UserManager<CampUser> userManager) : base(logger, mapper, repository, userManager)
        {

        }

        public override IActionResult GetWithCount(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? Repository.GetSpeakersByMonikerWithTalks(moniker) : Repository.GetSpeakersByMoniker(moniker);

            return Ok(new
            {
                currentTime = DateTime.UtcNow,
                count = speakers.Count(),
                results = Mapper.Map<IEnumerable<Speaker2Model>>(speakers)
            });
        }
    }
}
