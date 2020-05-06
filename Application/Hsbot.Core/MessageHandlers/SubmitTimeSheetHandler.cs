using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class SubmitTimeSheetHandler : MessageHandlerBase
    {
        private Regex regex = new Regex("submit ?(.*) time", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string[] CannedResponses => new[]
        {
            "https://media.giphy.com/media/A5RlxnMGaf5h6bBdsz/giphy.gif", // Danny DeVito
            "https://media.giphy.com/media/QNv4mKyspa8j6/giphy.gif", // Mad Men
            "https://media.giphy.com/media/mD6u5lgvneVag/giphy.gif", // Terry Tate
            "https://imgflip.com/gif/2ipx84", // Elf Elevator
            "https://makeagif.com/i/cOFbWG", // Steve Ballmer
            "https://i.imgur.com/V1gXxhL.gif", // Kid dancing on table
            "https://i.imgflip.com/372a6i.jpg", // Ash Ketchem!
            "https://i.imgflip.com/2zdpz4.jpg", // Traffic Cop, Dumb & Dumber
            "https://clockify.me/blog/wp-content/uploads/2019/01/timesheet-meme-20.jpg", // Oprah timesheet
            "https://i.imgflip.com/34h1my.jpg", // Keanu Reeves breathtaking
            "https://i.pinimg.com/originals/f1/30/c9/f130c97120ee10d752ce3b87435c6dcf.png", // The Beek
            "https://clockify.me/blog/wp-content/uploads/2019/01/timesheet-meme-51.jpg", // Braveheart
            "https://www.memesmonkey.com/images/memesmonkey/b3/b373da7676bdb1452b0f699580bf8587.jpeg", // Tennis
            "https://memecrunch.com/meme/4GV8G/bill-murray-timesheet/image.jpg?w=400&c=1", // Bill Murray
            "https://clockify.me/blog/wp-content/uploads/2019/01/timesheet-meme-33.jpg", // most interesting timesheet in the world
            "https://i.imgur.com/0PtfuRw.png", // Zoolander, so hot right now
            "https://i.pinimg.com/236x/bb/e5/20/bbe52050a7550351e4e66d4c56595ced--meme-batman-work-memes.jpg", // Batman hero
            "https://i.imgflip.com/urgdm.jpg", // 99 problems but a timesheet ain't one
            "https://i.imgur.com/mA7ELwI.jpg", // Star Wars
            "http://m.quickmeme.com/img/85/85a715b42634892b459abb4352eb5314030d546fdca072e11154012d10a3544d.jpg", // Evil Raccoon
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTxrS5kv5Bo85Dz-1W2FMXX6Sk7CeQDiORgPxwvo3nXUDeE14J6&s", // Finn
            "https://www.memecreator.org/static/images/memes/4059437.jpg", // Timesheets in yo timesheets
            "https://memecreator.org/static/images/memes/4150884.jpg", // Lumberg
            "https://blog-cdn.everhour.com/blog/wp-content/uploads/2019/09/4.0.0-2.jpg", // Rocky Horror
            "https://blog-cdn.everhour.com/blog/wp-content/uploads/2019/09/fill-out-your-x83yh5-1-768x432.jpg", // The Office
            "https://blog-cdn.everhour.com/blog/wp-content/uploads/2019/09/images-1.jpeg" // Geico Camel
        };

        public SubmitTimeSheetHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => new[] {"#headspring", "#hsbottesting"};
        public override bool DirectMentionOnly => false;

        public override double GetHandlerOdds(InboundMessage message)
        {
            return 0.67;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(regex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            await context.SendResponse(GetRandomCannedResponse());
        }
    }
}
