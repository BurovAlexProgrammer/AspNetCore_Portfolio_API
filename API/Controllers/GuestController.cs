using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebDAL.Entity;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GuestController : ControllerBase
    {
        private ILogger<GuestController> _logger;

        public GuestController(ILogger<GuestController> logger)
        {
            _logger = logger;
        }
        
        [HttpPost]
        public IActionResult Login([FromBody]object json)
        {
            var guestInput = System.Text.Json.JsonSerializer.Deserialize<Guest>(json.ToString()!);
            var existGuest = GetGuest(guestInput.name);

            if (existGuest == null)
            {
                return NotFound($"Гость с именем '{guestInput.name}' не найден.");
            }

            if (existGuest.password != guestInput.password)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "Неверный пароль.");
            }
            
            return new ObjectResult(existGuest);
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody]object jsonGuest)
        {
            var guestInput = System.Text.Json.JsonSerializer.Deserialize<Guest>(jsonGuest.ToString()!);
            return new OkObjectResult(IsCorrectToken(guestInput)); 
        }

        [NonAction]
        private Guest GetGuest(string name)
        {
            using (var db = new PdbContext())
            {
                var guests = db.Guests.Where(x => x.name == name).ToList();

                if (guests.Count > 1)
                {
                    _logger.LogError($"db.Guest - exc: duplicate guest name '{name}'");
                    return null;
                }

                if (guests.Count == 0)
                {
                    _logger.LogError($"db.Guest - guest '{name}' not found.");
                    return null;
                }

                var result = guests.First();
                result.token = Guid.NewGuid().ToString();
                db.Update(result);
                
                return guests.First();
            }
        }

        [NonAction]
        private bool IsCorrectToken(Guest guest)
        {
            using (var db = new PdbContext())
            {
                return db.Guests.Any(x => x.token == guest.token && x.name == guest.name);
            }
        }
        
    }
}