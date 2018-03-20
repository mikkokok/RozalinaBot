using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RozalinaBot.Helpers;

namespace RozalinaBot.Collectors.Ouman
{
    internal class OumanCollector
    {
        private string _url;
        private readonly string _polledUrl;
        private readonly TimeSpan _starTimeSpan;
        private readonly TimeSpan _fiveMinTimeSpan;
        private Timer _timer;
        public string LastResult { get; private set; }

        public OumanCollector(string oumanurl)
        {
            _polledUrl = oumanurl;
            _starTimeSpan = TimeSpan.Zero;
            _fiveMinTimeSpan = TimeSpan.FromMinutes(5);
            StartPolling();
        }

        public void StartPolling()
        {
            _timer = new Timer(async e =>
            {
                LastResult = await GetAsync(_polledUrl);
            }, null, _starTimeSpan, _fiveMinTimeSpan);
        }

        public async Task<string> GetAsync(string address)
        {
            var sb = new StringBuilder();
            try
            {
                _url = $"{address}request?S_227_85;S_1000_0;S_261_85;S_278_85;S_259_85;S_275_85;S_102_85;S_284_85;S_274_85;S_272_85;S_26_85;";
                var result = await DoRequestAsync(_url);
                if (string.IsNullOrEmpty(result))
                    return "";
                var split = result.Split('?')[1].Split(';');
                foreach (var splitted in split)
                {
                    sb.Append(Translate(splitted));
                    sb.Append("\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                sb.Append($"An error occurred when querying {_url}");
            }
            return sb.ToString();
        }
        private static async Task<string> DoRequestAsync(string uri)
        {
            string results;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidator.ValidateSslCertificate;
            
            request.Credentials = new NetworkCredential(AppLoader.LoadedConfig.OumanUser, AppLoader.LoadedConfig.OumanPassword);
            request.PreAuthenticate = true;
            using (var resp = await request.GetResponseAsync())
            {
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    results = await sr.ReadToEndAsync();
                }
            }
            return results;
        }
        private static string Translate(string setwithcode)
        {
            if (string.IsNullOrEmpty(setwithcode) || !setwithcode.Contains("=")) return "";
            var code = setwithcode.Split('=')[0];
            var result = setwithcode.Split('=')[1];
            string translation;
            if (code.Equals("S_227_85"))
                translation = "Ulkolämpötila";
            else if (code.Equals("S_261_85"))
                translation = "Mitattu huonelämpötila";
            else if (code.Equals("S_278_85"))
                translation = "Säätimen määräämä huonelämpötila";
            else if (code.Equals("S_259_85"))
                translation = "Mitattu L1 menoveden lämpötila";
            else if (code.Equals("S_275_85"))
                translation = "Säätimen määräämä menoveden lämpötila";
            else if (code.Equals("S_275_85"))
                translation = "Säätimen määräämä menoveden lämpötila";
            else if (code.Equals("S_102_85"))
                translation = "Huonelämpötilan hienosäätö";
            else if (code.Equals("S_1000_0"))
                translation = "Lämpötaso";
            else if (code.Equals("S_274_85"))
                translation = "Huonelämpökaukoasetus TMR/SP";
            else if (code.Equals("S_284_85"))
                translation = "L1 Huonelämpötila";
            else if (code.Equals("S_272_85"))
                translation = "L1 Venttiilin asento";
            else if (code.Equals("S_26_85"))
                translation = "Trendin näytteenottoväli";
            else if (code.Equals("S_1000_0"))
                translation = "Lämpötaso";
            else
                translation = code;

            return $"{translation} = {result}";
        }
    }
}
