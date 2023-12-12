using Newtonsoft.Json.Linq;
using SearchForDriversWebApp.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchForDriversWebApp
{
    public class BingMapsService : IBingMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "Aq_MSVDpYrh0iUMq3N9xz3xt75jbitM8hyJ2ImBWnr2njhflM26HsSFsA2XPLTX5";

        public BingMapsService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GetAddressFromCoordinates(double latitude, double longitude)
        {
            string apiUrl = $"http://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?key={_apiKey}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(jsonString);
                return data.resourceSets[0].resources[0].address.formattedAddress;
            }

            return null;
        }

        public async Task<double> CalculateDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
        {
            string apiUrl = $"http://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?key={_apiKey}" +
                              $"&origins={startLatitude},{startLongitude}&destinations={endLatitude},{endLongitude}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(jsonString);
                double distanceInKm = data.resourceSets[0].resources[0].results[0].travelDistance;
                return distanceInKm;
            }

            return 0;
        }
    }
}
