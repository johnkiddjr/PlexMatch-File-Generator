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

        public static T CreateAndGetRestResponse<T>(RestClient client, string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            var response = client.ExecuteGetAsync<T>(request);

            response.Wait(5000);

            return response.Result.Data;
        }
    }
}
