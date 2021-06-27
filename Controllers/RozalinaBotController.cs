using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RozalinaBot.InfoDeployers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RozalinaBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RozalinaBotController : ControllerBase
    {
        private IConfiguration _config;
        private IRozalinaBot _rozabot;
        public RozalinaBotController(IConfiguration configuration, IRozalinaBot rozaBot)
        {
            _config = configuration;
            _rozabot = rozaBot;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<IActionResult> Post(string message, string from, bool admin)
        {
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
