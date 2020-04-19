using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using TestApi;

namespace WebApiTest.Helpers
{
    public static class TestHelper
    {
        internal static async Task<T> DeserializeResponseData<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        internal static TestServer ArcTestServer
            => new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Test"));
    }
}