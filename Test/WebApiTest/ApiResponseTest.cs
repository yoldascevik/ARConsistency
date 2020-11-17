using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ARConsistency.Abstractions;
using Newtonsoft.Json;
using TestApi.Models;
using WebApiTest.Helpers;
using Xunit;

namespace WebApiTest
{
    public class ApiResponseTest
    {
        private readonly HttpClient _client;

        public ApiResponseTest()
        {
            _client = TestHelper.ArcTestServer.CreateClient();
        }

        [Fact]
        public async void ApiResponseStringResult_Should_Accepted202_ExpectedStringResult()
        {
            // Arrange
            var expectedResult = "ApiResponseStringResult method called and http status code set to 202 (Accepted).";

            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/ApiResponseStringResult");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);
            var payloadString = consistentApiResponse.Payload.ToString();

            // Assert
            Assert.Equal(202, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.Equal(expectedResult, payloadString);
        }

        [Fact]
        public async void ApiResponseArrayResult_Should_Accepted202_FiveItems()
        {
            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/ApiResponseArrayResult");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);
            var payload =
                JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(consistentApiResponse.Payload.ToString());

            // Assert
            Assert.Equal(202, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.Equal(5, payload.Count());
        }
    }
}