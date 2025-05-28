using eShopLegacyMVC.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eShopLegacyMVC.Services
{
    public class WeatherService
    {
        const int DefaultZipCode = 98052;
        const string RequestFormatString = "http://api.weatherapi.com/v1/current.json?key={0}&q={1}&aqi=no";
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public WeatherService(IConfiguration configuration, HttpClient httpClient = null)
        {
            _apiKey = configuration.GetValue<string>("AppSettings:weather:ApiKey") ?? 
                      Environment.GetEnvironmentVariable("WEATHER_API_KEY");
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<int?> GetUserCurrentTemperatureAsync(ApplicationUser user, bool celsius)
        {
            var zipCode = user?.ZipCode ?? DefaultZipCode;
            return await GetTemperatureAsync(zipCode, celsius);
        }

        private async Task<int?> GetTemperatureAsync(int zipCode, bool celsius)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(string.Format(RequestFormatString, _apiKey, zipCode));
                
                if (!string.IsNullOrEmpty(response))
                {
                    var weatherData = JsonConvert.DeserializeAnonymousType(response, new { Current = new { Temp_C = 0, Temp_F = 0 } });
                    if (weatherData != null)
                    {
                        return celsius ? weatherData.Current.Temp_C : weatherData.Current.Temp_F;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
