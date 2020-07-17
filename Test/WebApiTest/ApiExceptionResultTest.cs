using System.Net;
using System.Net.Http;
using ARConsistency.ResponseModels.Base;
using TestApi.Exceptions;
using WebApiTest.Helpers;
using Xunit;

namespace WebApiTest
{
    public class ApiExceptionResultTest
    {
        private readonly HttpClient _client;

        public ApiExceptionResultTest()
        {
            _client = TestHelper.ArcTestServer.CreateClient();
        }

        [Fact]
        public async void UnHandledExceptionResult_Should_InternalServerError500_ExpectedExceptionWithDetails()
        {
            // Arrange
            var expectedExceptionMessage = "test";
            var expectedMessage = "Unhandled Exception occurred. Unable to process the request.";

            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/UnHandledExceptionResult");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);

            // Assert
            Assert.Equal(500, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(expectedMessage, consistentApiResponse.Message);
            Assert.Equal(expectedExceptionMessage, consistentApiResponse.ExceptionMessage);
            Assert.True(string.IsNullOrEmpty(consistentApiResponse.ExceptionDetails));
        }
        
        [Fact]
        public async void ThrowApiException_Should_BadRequest400_ExpectedExceptionMessage()
        {
            // Arrange
            var expectedExceptionMessage = "ThrowApiException method called!";

            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/ThrowApiException");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);

            // Assert
            Assert.Equal(400, consistentApiResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedExceptionMessage, consistentApiResponse.ExceptionMessage);
        }
        
        [Fact]
        public async void ItemNotFoundException_Should_StatusCodedException_ExpectedStatusCode()
        {
            // Arrange
            var expectedStatusCode = new ItemNotFoundException().StatusCode;

            // Act
            var response = await _client.GetAsync("/api/WeatherForecast/ItemNotFoundException");
            var consistentApiResponse = await TestHelper.DeserializeResponseData<ConsistentApiResponse>(response);

            // Assert
            Assert.Equal(expectedStatusCode, consistentApiResponse.StatusCode);
        }
    }
}