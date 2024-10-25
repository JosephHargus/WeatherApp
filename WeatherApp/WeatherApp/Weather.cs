using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;

namespace WeatherApp
{
    //define classes to represent the JSON structure
    public class WeatherData
    {
        private const String ApiBaseUrl = @"http://api.weatherapi.com/v1";
        private const String Key = "7c00342fb83948cc958204236241904";

        [JsonProperty("location")]
        public Location? Location { get; set; }

        [JsonProperty("current")]
        public Current? Current { get; set; }

        [JsonProperty("forecast")]
        public Forecast? Forecast { get; set; }

        [JsonProperty("alerts")]
        public Alerts? Alerts { get; set; }

        public async Task GetWeatherDataAsync(String location = "38.9478,-92.3269")
        {
            try
            {
                //create client
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ApiBaseUrl);

                //get API response
                string endpoint = @"/forecast.json";
                HttpResponseMessage response = await client.GetAsync($"{ApiBaseUrl}{endpoint}?key={Key}&q={location}&days=5&aqi=no&alerts=yes");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully retrieved weather data.", "Success", MessageBoxButton.OK);

                    //deserialize API response
                    String jsonResponse = await response.Content.ReadAsStringAsync();
                    WeatherData? weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse);

                    if (weatherData != null)
                    {
                        Location = weatherData.Location;
                        Current = weatherData.Current;
                        Forecast = weatherData.Forecast;
                        Alerts = weatherData.Alerts;

                        return;
                    }

                }
                //error occurred
                throw new Exception();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve weather data.\nPlease refresh to try again.\nException: " + ex.Message, "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                Location = null;
                Current = null;
                Forecast = null;
                Alerts = null;
            }
        }
    }
}