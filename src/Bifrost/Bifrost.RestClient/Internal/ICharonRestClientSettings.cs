namespace Bifrost.RestClient.Internal
{
    public interface IAsgardRestClientSettings
    {
        string AsgardApiUrl { get; set; }
        int AsgardTimeoutMS { get; set; }

        /// <summary>
        /// To be implemented..
        /// </summary>
        bool UseDefaultProxy { get; set ; }

        string AsgardClientApiKey { get; set; }
    }
}