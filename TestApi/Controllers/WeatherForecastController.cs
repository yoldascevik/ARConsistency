﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ARConsistency.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

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

        [HttpGet]
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
        [Route("ThrowApiException")]
        public bool ThrowApiException()
        {
            throw new ApiException("ThrowApiException method called!");
        }
    
        #endregion
    }
}
