using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class WhereIsRoomMessageHandler : MessageHandlerBase
    {
        private const string CommandText = "where is";

        private readonly Dictionary<string, string> _rooms = new Dictionary<string, string>
        {
            //Austin
            {"mercury", "https://i.imgur.com/6GyUimi.png"},
            {"zinc", "https://i.imgur.com/erKXXNJ.png"},
            {"silver", "https://i.imgur.com/YmA07P3.png"},
            {"titanium", "https://i.imgur.com/lrSteNN.png"},
            {"oxygen", "https://i.imgur.com/KotsrgR.png"},
            {"hydrogen", "https://i.imgur.com/rtr6Yf8.png"},
            {"silicon", "https://i.imgur.com/TI7o6HV.png"},
            {"carbon", "https://i.imgur.com/EszbFwm.png"},
            {"nitrogen", "https://i.imgur.com/ky4pgBK.png"},
            {"promethium", "https://i.imgur.com/1S6ieRs.png"},
            {"mean-eyed cat", "https://i.imgur.com/lGimPD5.png"},
            {"mean eyed cat", "https://i.imgur.com/lGimPD5.png"},
            {"lustre pearl", "https://i.imgur.com/nf6jOSt.png"},

            //Not rooms
            {"coffee", ":jura:"},
            {"jimmy", "https://s3.amazonaws.com/grabbagoftimg/jimmy.png"},

            //Houston
            {"burnet", "http://i.imgur.com/CUJd3Di.png"},
            {"capital of texas", "http://i.imgur.com/20GipJn.png"},
            {"mopac", "http://i.imgur.com/YrVXsQA.png"},
            {"morado", "http://i.imgur.com/4Uc5c2G.png"},
            {"richmond", "http://i.imgur.com/XEv0cEw.png"},
            {"spicewood", "http://i.imgur.com/UMxipTc.png"}
        };

        private readonly List<string> _barks = new List<string>
        {
            "I only know how to find conference rooms.",
            "I don't think {0} is a conference room.",
            "I dunno..."
        };

        public WhereIsRoomMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor
                {
                    Command = CommandText,
                    Description = "Shows where a room is in the office."
                }
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith(CommandText);
        }

        private string GetRandomBark()
        {
            var random = new System.Random();
            return _barks[random.Next(0, _barks.Count)];
        }

        public override Task HandleAsync(IBotMessageContext context)
        {
            var match = context.Message.Match(new Regex(@"where ?[i']s ([^\?]*)[\?]*"));

            if (match.Value == string.Empty || match.Groups.Count < 2)
            {
                return ReplyToChannel(context, "Gimme a room name to look for!");
            }

            var roomSearch = match.Groups[1].Value.ToLower();

            if (!_rooms.ContainsKey(roomSearch))
            {
                return ReplyToChannel(context, string.Format(GetRandomBark(), roomSearch));
            }

            var room = _rooms[roomSearch];

            if (room.StartsWith("http"))
            {
                return ReplyToChannel(context, null, new Attachment
                {
                    ImageUrl = room
                });
            }

            return ReplyToChannel(context, room);
        }
    }
}
