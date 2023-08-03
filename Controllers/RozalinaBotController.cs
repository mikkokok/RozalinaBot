using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RozalinaBot.Helpers;
using RozalinaBot.InfoDeployers;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RozalinaBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RozalinaBotController : ControllerBase
    {
        private IConfiguration _config;
        private IRozalinaBot _rozabot;
        private ApikeyValidator _apikeyValidator;
        public RozalinaBotController(IConfiguration configuration, IRozalinaBot rozaBot)
        {
            _config = configuration;
            _rozabot = rozaBot;
            _apikeyValidator = new ApikeyValidator(_config);
        }
        [HttpGet]
        public async Task<IActionResult> Get(string apikey, string feature)
        {
            if (!_apikeyValidator.validateApiKey(apikey))
            {
                return Unauthorized();
            }

            if (feature == "doorbell")
            {
                await _rozabot.SendDoorBellPicture();
            }
            if (feature == "diaper")
            {
                _rozabot.addDiaperChange();
            }
            if (feature == "boilerroomflood")
            {
                await _rozabot.SendToAll("Flood detected in boiler room");
            }
            if (feature == "kitchenflood")
            {
                await _rozabot.SendToAll("Flood detected in kitchen");
            }


            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string apikey, string message, string from, bool admin)
        {
            if (!_apikeyValidator.validateApiKey(apikey))
            {
                return Unauthorized();
            }

            if (admin)
            {
                await _rozabot.SendAdminMessages(message, from);
                return Ok();
            }
            await _rozabot.SendToAll(message, from);
            return Ok();
        }
    }
}
