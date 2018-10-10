using Hls.Proxy.Aes.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Hls.Proxy.Aes.Controllers
{
    [Route("api/ManifestLoad")]
    public class ManifestLoadController : Controller
    {
        private const string ManifestProxyUrlTemplate = "http://{0}:{1}/api/ManifestProxy";

        private readonly ITopLevelManifestRetriever _topLevelManifestRetriever;

        public ManifestLoadController(ITopLevelManifestRetriever topLevelManifestRetriever)
        {
            _topLevelManifestRetriever = topLevelManifestRetriever;
        }

        public ActionResult Get(string playbackUrl, string webtoken)
        {
            if (playbackUrl == null || webtoken == null)
                return BadRequest("playbackUrl or webtoken cannot be empty");
            var token = webtoken;

            var modifiedTopLeveLManifest = _topLevelManifestRetriever.GetTopLevelManifestForToken(GetManifestProxyUrl(), playbackUrl, token);

            var response = new ContentResult
            {
                Content = modifiedTopLeveLManifest,
                ContentType = @"application/vnd.apple.mpegurl"
            };
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("Cache-Control", "max-age=259200");

            return response;
        }

        private string GetManifestProxyUrl()
        {
            if (Request.GetUri() == null) return string.Empty;
            var hostPortion = Request.GetUri().Host;
            var port = Request.GetUri().Port == 80 ? string.Empty : Request.GetUri().Port.ToString();
            var manifestProxyUrl = string.Format(ManifestProxyUrlTemplate, hostPortion, port);
            return manifestProxyUrl;
        }
    }
}