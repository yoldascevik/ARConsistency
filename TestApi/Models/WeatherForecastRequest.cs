using System;
using System.ComponentModel.DataAnnotations;

namespace TestApi.Models
{
    public class WeatherForecastRequest
    {
        [Required(ErrorMessage = "CountryCode field is required for WeatherForecast")]
        public string CountryCode { get; set; }
        public DateTime WeatherForecastDate { get; set; }
    }
}