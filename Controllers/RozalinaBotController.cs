using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public RozalinaBotController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };

        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
