using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class FlipMessageHandler : MessageHandlerBase
    {
        private const string FlipResponse = @"(╯°□°）╯︵ {0}";
        private const string MegaflipResponse = @"(╯°□°）╯︵
┳┳┳┳┳┳　　|
┓┏┓┏┓┃　{0}
┛┗┛┗┛┃
┓┏┓┏┓┃
┛┗┛┗┛┃
┓┏┓┏┓┃
┛┗┛┗┛┃
┓┏┓┏┓┃
┻┻┻┻┻┻";

        private static readonly Regex FlipRegex = new Regex("^flip (.+)", RegexOptions.Compiled);
        private static readonly Regex MegaflipRegex = new Regex("^megaflip (.+)", RegexOptions.Compiled);

        public FlipMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor {Command = "flip X", Description = "Flips the text X"},
                new MessageHandlerDescriptor {Command = "megaflip X", Description = "Mega-flips the text X"}
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(FlipRegex) || message.IsMatch(MegaflipRegex);
        }

        public override Task HandleAsync(InboundMessage message)
        {
            var match = message.Match(FlipRegex);
            var responseText = FlipResponse;

            if (!match.Success)
            {
                match = message.Match(MegaflipRegex);
                responseText = MegaflipResponse;
            }

            var flippedText = FlipText(match.Groups[1].Value);
            return SendMessage(message.CreateResponse(string.Format(responseText, flippedText)));
        }

        private string FlipText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var flippedArray = new char[text.Length];
            for (var i = 0; i < text.Length; ++i)
            {
                flippedArray[i] = FlipChar(text[i]);
            }

            return new string(flippedArray);
        }

        private char FlipChar(char ch)
        {
            switch (ch)
            {
                case 'a': return '\u0250';
                case 'b': return 'q';
                case 'c': return '\u0254';
                case 'd': return 'p';
                case 'e': return '\u01DD';
                case 'f': return '\u025F';
                case 'g': return 'b';
                case 'h': return '\u0265';
                case 'i': return '\u0131';
                case 'j': return '\u0638';
                case 'k': return '\u029E';
                case 'l': return '\u05DF';
                case 'm': return '\u026F';
                case 'n': return 'u';
                case 'o': return 'o';
                case 'p': return 'd';
                case 'q': return 'b';
                case 'r': return '\u0279';
                case 's': return 's';
                case 't': return '\u0287';
                case 'u': return 'n';
                case 'v': return '\u028C';
                case 'w': return '\u028D';
                case 'x': return 'x';
                case 'y': return '\u028E';
                case 'z': return 'z';
                case '[': return ']';
                case ']': return '[';
                case '(': return ')';
                case ')': return '(';
                case '{': return '}';
                case '}': return '{';
                case '?': return '\u00BF';
                case '\u00BF': return '?';
                case '\'': return ',';
                case '.': return '\u02D9';
                case '_': return '\u203E';
                case ';': return '\u061B';
                case '9': return '6';
                case '6': return '9';
                case '\u0250': return 'a';
                case '\u0254': return 'c';
                case '\u01DD': return 'e';
                case '\u025F': return 'f';
                case '\u0265': return 'h';
                case '\u0131': return 'i';
                case '\u0638': return 'j';
                case '\u029E': return 'k';
                case '\u05DF': return 'l';
                case '\u026F': return 'm';
                case '\u0279': return 'r';
                case '\u0287': return 't';
                case '\u028C': return 'v';
                case '\u028D': return 'w';
                case '\u028E': return 'y';
                case ',': return '\'';
                case '\u02D9': return '.';
                case '\u203E': return '_';
                case '\u061B': return ';';
                default: return ch;
            }
        }
    }
}
