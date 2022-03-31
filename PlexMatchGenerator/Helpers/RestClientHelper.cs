using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace PlexMatchGenerator.Helpers
{
    public class RestClientHelper
    {
        public static RestClient GenerateClient(string plexUrl, string plexToken)
        {
            var client = new RestClient(plexUrl);
            client.UseNewtonsoftJson();
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("X-Plex-Token", plexToken);

            return client;
        }

        public static async Task<T> CreateAndGetRestResponse<T>(RestClient client, string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            var response = await client.ExecuteGetAsync<T>(request);

            return response.Data;
        }
    }
}
