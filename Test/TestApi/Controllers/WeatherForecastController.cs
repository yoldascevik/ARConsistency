using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARConsistency.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApi.Exceptions;
using TestApi.Models;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet]
        [Route("BySummary/{summary}")]
        public IActionResult GetBySummary(string summary)
        {
            return Ok(Get().FirstOrDefault(x=> x.Summary == summary));
        }
        
        #region ApiResponseResults
        [HttpGet]
        [Route("ApiResponseStringResult")]
        public IActionResult ApiResponseStringResult()
        {
            return new ApiResponse("ApiResponseStringResult method called and http status code set to 202 (Accepted).", StatusCodes.Status202Accepted);
        }

        [HttpGet]
        [Route("ApiResponseArrayResult")]
        public IActionResult ApiResponseArrayResult()
        {
            return new ApiResponse(Get(), StatusCodes.Status202Accepted);
        }
        #endregion

        #region ApiErrorResults
        [HttpGet]
        [Route("ApiErrorStringResult")]
        public IActionResult ApiErrorStringResult()
        {
            return new ApiError("ApiErrorStringResult method: test error message!");
        }

        [HttpPost]
        [Route("ApiErrorValidationErrorResult")]
        public IActionResult ApiErrorValidationErrorResult([FromBody]WeatherForecastRequest request)
        {
            if (!ModelState.IsValid)
                return null;

            return new ApiResponse(Get(), StatusCodes.Status202Accepted);
        }
        #endregion

        #region ApiExceptionResults
        [HttpGet]
        [Route("UnHandledExceptionResult")]
        public bool UnHandledExceptionResult()
        {
            throw new DivideByZeroException("test");
        }
        
        [HttpGet]
        [Route("ItemNotFoundException")]
        public bool ItemNotFoundException()
        {
            throw new ItemNotFoundException("ItemNotFoundException method called!");
        }

        [HttpGet]
        [Route("ThrowApiException")]
        public bool ThrowApiException()
        {
            throw new ApiException("ThrowApiException method called!");
        }
    
        #endregion
        
        #region Void & Task Actions

        // POST api/WeatherForecast/TestVoid
        [HttpPost("TestVoid")]
        public void TestVoid() { }

        // POST api/WeatherForecast/TestTaskAsync
        [HttpPost("TestTaskAsync")]
        public async Task TestTaskAsync()
        {
            await Task.CompletedTask;
        }

        #endregion

    }
}
