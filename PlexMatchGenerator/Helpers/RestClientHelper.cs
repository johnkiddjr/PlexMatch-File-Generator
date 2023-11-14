using PlexMatchGenerator.Constants;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace PlexMatchGenerator.Helpers
{
    public class RestClientHelper
    {
        public static RestClient GenerateClient(string plexUrl, string plexToken)
        {
            var client = new RestClient(plexUrl, configureSerialization: s => s.UseNewtonsoftJson());
            client.AddDefaultHeader(KnownHeaders.Accept, HttpConstants.ApplicationJson);
            client.AddDefaultHeader(HttpConstants.PlexTokenHeaderName, plexToken);

            return client;
        }

        public static async Task<T> CreateAndGetRestResponse<T>(RestClient client, string resource, Method method, Dictionary<string, string> additionalHeaders = null)
        {
            var request = new RestRequest(resource, method);

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            var response = await client.ExecuteGetAsync<T>(request);

            return response.Data;
        }
    }
}
