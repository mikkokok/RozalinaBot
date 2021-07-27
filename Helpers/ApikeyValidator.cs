using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RozalinaBot.Helpers
{
    public class ApikeyValidator
    {
        private IConfiguration _config;
        private string _apikey;
        public ApikeyValidator(IConfiguration config)
        {
            _config = config;
            _apikey = _config["ApiKey"];
        }
        public bool validateApiKey(string apiKey)
        {
            return apiKey.Equals(_apikey);
        }
    }
}
