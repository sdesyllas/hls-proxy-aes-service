using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Web;
using Hls.Proxy.Aes.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hls.Proxy.Aes.Controllers
{
    [Produces("application/vnd.apple.mpegurl")]
    [Route("api/ManifestProxy")]
    public class ManifestProxyController : Controller
    {
        private readonly ITokenManifestInjector _tokenManifestInjector;

        public ManifestProxyController(ITokenManifestInjector tokenManifestInjector)
        {
            _tokenManifestInjector = tokenManifestInjector;
        }

        public IActionResult Get(string playbackUrl, string token)
        {
            var collection = HttpUtility.ParseQueryString(token);
            var authToken = collection[0];
            var armoredAuthToken = HttpUtility.UrlEncode(authToken);

            var httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(playbackUrl));
            httpRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            httpRequest.Timeout = 30000;
            var httpResponse = httpRequest.GetResponse();
            
            //var response = this.Request.ReadFormAsync().Result;
            var response = new ContentResult();
            try
            {
                var stream = httpResponse.GetResponseStream();
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();
                        response.Content =
                            _tokenManifestInjector.InjetTokenToManifestChunks(playbackUrl, armoredAuthToken, content);
                    }
                }
            }
            finally
            {
                httpResponse.Close();
            }
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }
    }
}