namespace Hsbot.Core.Maps
{
    using Newtonsoft.Json;

    public class DirectionResponse
    {
        public string Status { get; set; }
        public DirectionRoute[] Routes { get; set; }
    }

    public class DirectionRoute
    {
        public DirectionRouteLeg[] Legs { get; set; }
        [JsonProperty(PropertyName = "overview_polyline")]
        public DirectionRouteOverViewPolyline OverviewPolyline { get; set; }
        public string Summary { get; set; }
    }

    public class DirectionRouteOverViewPolyline
    {
        public string Points { get; set; }
    }

    public class DirectionRouteLeg
    {
        [JsonProperty(PropertyName = "start_address")]
        public string StartAddress { get; set; }
        [JsonProperty(PropertyName = "end_address")]
        public string EndAddress { get; set; }
        public DirectionRouteLegTextValue Distance { get; set; }
        public DirectionRouteLegTextValue Duration { get; set; }
        public DirectionRouteLegStep[] Steps { get; set; }
    }

    public class DirectionRouteLegStep
    {
        [JsonProperty(PropertyName = "html_instructions")]
        public string HtmlInstructions { get; set; }
        public string Maneuver { get; set; }
        public DirectionRouteLegTextValue Distance { get; set; }
        public DirectionRouteLegTextValue Duration { get; set; }
    }

    public class DirectionRouteLegTextValue
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
