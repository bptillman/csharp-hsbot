namespace Hsbot.Core.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Maps;
    using Messaging;
    using Random;

    public class MapsMessageHandler : MessageHandlerBase
    {
        private const string MapMeCommand = "map me";
        private const string DirectionsFromCommand = "directions from";

        private static readonly Regex MapMeRegex = new Regex(@"((roadmap|satellite|terrain|hybrid) )?map me (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex DirectionsFromRegex = new Regex(@"((driving|walking|bike|biking|bicycling) )?directions from (.*) to (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IMapProvider _maps;

        public MapsMessageHandler(IRandomNumberGenerator randomNumberGenerator, IMapProvider maps) : base(randomNumberGenerator)
        {
            _maps = maps;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor { Command = MapMeCommand + " <query>", Description = "Returns a map view of the area returned by 'query'."},
                new MessageHandlerDescriptor { Command = DirectionsFromCommand + " <origin> to <destination>", Description = "Returns a map and list of directions from origin to destination."},
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            var match = message.Match(MapMeRegex);
            if (match.Success &&
                message.StartsWith(MapMeCommand) ||
                match.Groups[2].Value.Length > 0 && message.StartsWith(match.Groups[2].Value))
            {
                return true;
            }

            match = message.Match(DirectionsFromRegex);
            return match.Success &&
                   message.StartsWith(DirectionsFromCommand) ||
                   match.Groups[2].Value.Length > 0 && message.StartsWith(match.Groups[2].Value);
        }

        public override async Task HandleAsync(InboundMessage context)
        {
            await HandleMapMeCommand(context);
            await HandleDirectionsFromCommand(context);
        }

        private async Task HandleMapMeCommand(InboundMessage message)
        {
            var match = message.Match(MapMeRegex);
            if (match.Success &&
                message.StartsWith(MapMeCommand) ||
                match.Groups[2].Value.Length > 0 && message.StartsWith(match.Groups[2].Value))
            {
                var query = match.Groups[3].Value;
                var mapType = Enum.TryParse(match.Groups[2].Value, true, out MapType parsedMapType) ? parsedMapType : MapType.Roadmap;

                var map = _maps.GetMap(query, mapType);

                await SendMessage(message.CreateResponse(map.ImageUrl));
                await SendMessage(message.CreateResponse(map.Url));
            }
        }

        private async Task HandleDirectionsFromCommand(InboundMessage message)
        {
            var match = message.Match(DirectionsFromRegex);
            if (match.Success &&
                message.StartsWith(DirectionsFromCommand) ||
                match.Groups[2].Value.Length > 0 && message.StartsWith(match.Groups[2].Value))
            {
                await SendMessage(message.CreateTypingOnChannelResponse());

                var queryFrom = match.Groups[3].Value;
                var queryTo = match.Groups[4].Value;
                var directionsMode = Enum.TryParse(match.Groups[2].Value, true, out DirectionsMode parsedMapType) ? parsedMapType : DirectionsMode.Driving;

                MapDirections directions;
                try
                {
                    directions = await _maps.GetDirections(queryFrom, queryTo, directionsMode);
                }
                catch (Exception e)
                {
                    await SendMessage(message.CreateResponse(e.Message));
                    return;
                }

                await SendMessage(message.CreateResponse(directions.MapUrl));
                await SendMessage(message.CreateResponse(directions.Instructions));
            }
        }
    }
}