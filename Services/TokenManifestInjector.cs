using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hls.Proxy.Aes.Services
{
    public class TokenManifestInjector : ITokenManifestInjector
    {
        public string InjetTokenToManifestChunks(string playbackUrl, string armoredAuthToken, string content)
        {
            const string qualityLevelRegex = @"(QualityLevels\(\d+\))";
            const string fragmentsRegex = @"(Fragments\([\w\d=-]+,[\w\d=-]+\))";
            const string urlRegex =
                @"("")(https?:\/\/[\da-z\.-]+\.[a-z\.]{2,6}[\/\w \.-]*\/?[\?&][^&=]+=[^&=#]*)("")";
            var baseUrl = playbackUrl.Substring(0,
                              playbackUrl.IndexOf(".ism", StringComparison.OrdinalIgnoreCase)) +
                          ".ism";
            var newContent = Regex.Replace(content, urlRegex,
                string.Format(CultureInfo.InvariantCulture, "$1$2&token={0}$3", armoredAuthToken));
            var match = Regex.Match(playbackUrl, qualityLevelRegex);
            if (match.Success)
            {
                var qualityLevel = match.Groups[0].Value;
                newContent = Regex.Replace(newContent, fragmentsRegex,
                    m => string.Format(CultureInfo.InvariantCulture,
                        baseUrl + "/" + qualityLevel + "/" + m.Value));
            }

            return newContent;
        }
    }
}
