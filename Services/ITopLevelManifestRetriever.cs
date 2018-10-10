namespace Hls.Proxy.Aes.Services
{
    public interface ITopLevelManifestRetriever
    {
        string GetTopLevelManifestForToken(string manifestProxyUrl, string topLeveLManifestUrl, string token);
    }
}