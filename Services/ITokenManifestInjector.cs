namespace Hls.Proxy.Aes.Services
{
    public interface ITokenManifestInjector
    {
        string InjetTokenToManifestChunks(string playbackUrl, string armoredAuthToken, string content);
    }
}