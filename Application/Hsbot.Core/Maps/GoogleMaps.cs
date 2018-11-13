namespace Hsbot.Core.Maps
{
    using System;
    using System.Web;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using ApiClient;
    using Newtonsoft.Json;

    public class GoogleMaps : IMapProvider
    {
        private const string MapUrl = "http://maps.google.com/maps?t=m&z=11&hl=en&sll=37.0625,-95.677068&sspn=73.579623,100.371094&vpsrc=0";
        private const string ImageMapUrl = "http://maps.google.com/maps/api/staticmap?format=png&sensor=false&size=400x400";
        private const string DirectionsJsonUrl = "https://maps.googleapis.com/maps/api/directions/json?";
        private const string DirectionsMapUrl = "http://maps.googleapis.com/maps/api/staticmap?size=400x400&sensor=false&path=weight:3%7Ccolor:red%7Cenc:";

        private static readonly Regex RemoveDivTagsRegex = new Regex(@"<div[^>]*>", RegexOptions.Compiled);
        private static readonly Regex RemoveHtmlTagsRegex = new Regex(@"<[^>]*>", RegexOptions.Compiled);

        private readonly string _key;
        private readonly IApiClient _apiClient;

        public GoogleMaps(IHsbotConfig hsbotConfig, IApiClient apiClient)
        {
            _key = hsbotConfig.GoogleApiKey;
            _apiClient = apiClient;
        }

        public Map GetMap(string location, MapType mapType)
        {
            location = HttpUtility.UrlEncode(location);

            var mapUrl = $"{MapUrl}&q={location}&hnear={location}";
            var imageUrl = $"{ImageMapUrl}&markers={location}&maptype={mapType.ToString().ToLower()}";

            return new Map(mapUrl, imageUrl);
        }

        public async Task<MapDirections> GetDirections(string origin, string destination, DirectionsMode directionsMode)
        {
            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Now you're just being silly.");

            if (string.IsNullOrWhiteSpace(_key))
                throw new Exception("Please enter your Google API key in HsbotConfig google:apiKey.");

            if (directionsMode == DirectionsMode.Bike || directionsMode == DirectionsMode.Biking)
                directionsMode = DirectionsMode.Bicycling;

            var directions = await GetDirectionResponse(origin, destination, directionsMode);

            return GetMapDirections(directions);
        }

        private async Task<DirectionResponse> GetDirectionResponse(string origin, string destination, DirectionsMode directionsMode)
        {
            var parameters = new Dictionary<string, string>
            {
                {"mode", directionsMode.ToString().ToLower()},
                {"key", _key},
                {"origin", origin},
                {"destination", destination},
                {"sensor", "false"}
            };
            var directionsUrl = DirectionsJsonUrl + string.Join("&", parameters.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

            try
            {
                var directions = await _apiClient.RequestDeserializedJson<DirectionResponse>(HttpMethod.Get, directionsUrl);
                return directions;
            }
            catch (Exception)
            {
                throw new Exception("Error: Service is not working.");
            }
        }

        private static MapDirections GetMapDirections(DirectionResponse directions)
        {
            if (directions.Routes == null || directions.Routes.Length == 0)
                throw new Exception("Error: No route found.");

            var route = directions.Routes[0];
            var leg = route.Legs[0];
            var start = leg.StartAddress;
            var end = leg.EndAddress;
            var distance = leg.Distance.Text;
            var duration = leg.Duration.Text;

            var mapUrl = DirectionsMapUrl + route.OverviewPolyline.Points;

            var directionsResponse = $"Directions from {start} to {end}\n";
            directionsResponse += $"{distance} - {duration}\n\n";

            for (var i = 0; i < leg.Steps.Length; i++)
            {
                var instructions = leg.Steps[i].HtmlInstructions;
                instructions = RemoveDivTagsRegex.Replace(instructions, " - ");
                instructions = RemoveHtmlTagsRegex.Replace(instructions, "");

                directionsResponse += $"{i + 1}. {instructions} ({leg.Steps[i].Distance.Text})\n";
            }

            return new MapDirections(mapUrl, directionsResponse);
        }
    }
}