using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Services
{
    public class GeocodingServices : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodingServices(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Geocoding:ApiKey"];
        }

        public async Task<(double latitude, double longitude)> ObtenerCoordenadas(string direccion)
        {
            var encodedAddress = Uri.EscapeDataString(direccion);
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            var json = JObject.Parse(response);
            var results = json["results"];
            if (results== null || !results.HasValues)
            {
                throw new Exception("No se encontraron coordenadas para la direccion");
            }

            var location = results[0]["geometry"]["location"];
            double lat = (double)location["lat"];
            double lng = (double)location["lng"];
            return (lat, lng);
        }
    }
}
