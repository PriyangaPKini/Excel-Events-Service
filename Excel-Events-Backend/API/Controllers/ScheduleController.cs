using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Schedule;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [SwaggerTag("The routes under this controller are for event schedule.")]
    [Route("/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _repo;
        public ScheduleController(IScheduleRepository repo)
        {
            _repo = repo;
        }

        [SwaggerOperation(Description = "Event Schedule")]
        [HttpGet]   
        public async Task<List<EventForScheduleListViewDto>> EventList()
        {    
            var events =  await _repo.EventList();
            return events;
        }
    }
}