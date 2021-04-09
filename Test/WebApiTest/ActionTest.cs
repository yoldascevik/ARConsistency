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
    public class ActionTest
    {
        private readonly HttpClient _client;

        public ActionTest()
        {
            _client = TestHelper.ArcTestServer.CreateClient();
        }

        [Fact]
        public async void Get_Should_200Ok_FiveItems_IsErrorFalse()
        {
            // Act
            var response = await _client.GetAsync("/api/WeatherForecast");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);
            var payload = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(consistentApiResponse.Payload.ToString());

            // Assert
            Assert.Equal(200, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(5, payload.Count());
            Assert.False(consistentApiResponse.IsError);
        }

        [Fact]
        public async void TestVoid_Should_200Ok_IsErrorFalse()
        {
            // Act
            var response = await _client.PostAsync("/api/WeatherForecast/TestVoid", null);
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);

            // Assert
            Assert.Equal(200, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(consistentApiResponse.IsError);
        }

        [Fact]
        public async void GetBySummary_Should_200Ok_IsErrorFalse_PayloadIsNull_WithNotExistSummary()
        {
            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/BySummary/VeryHot");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);
            
            // Assert
            Assert.Equal(200, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(consistentApiResponse.IsError);
            Assert.Null(consistentApiResponse.Payload);
        }
        
        [Fact]
        public async void GetHtmlPage_ShouldNotBeConsistentResponse()
        {
            // Arrange
            string expectedContentType = "text/html";
            string expectedBodyString = "<div>ARConsistency Html Test Page!</div>";
            
            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/HtmlPage");
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseContentType = response.Content.Headers.ContentType.MediaType;
            
            // Assert
            Assert.Equal(expectedBodyString, responseBody);
            Assert.Equal(expectedContentType, responseContentType);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}