using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class TrafficMessageHandler : MessageHandlerBase
    {
        private readonly string _baseGoogleMapUrl = "https://www.google.com/maps/vt/data=";
        /// <summary>
        /// # To get data for dictionary:
        /// #  1. Do a google search for "<x> traffic"
        /// #  2. Right click the "Current traffic for <x>" image and 'Copy Image Address'
        /// #  3. We want the content after the data=
        /// </summary>
        private readonly Dictionary<string, string> _places = new Dictionary<string, string>
        {
            {"aus", "U-kLADtG82mzniX3qXRg0GngLxhB7XzXC0uMNSTwPo9P4_Z-iP1WGVmh4KcWTR-NsJFlq3YgPZWcf9hmnJyzJ-kaovbFNIpFb4VG5dAekJCLDP_KTLkykpTrfAjYuoEvso_NjtFWgeMqSIB80GKRtyWvwy3fJ1yzaRXfQyQB2kavzI3twLSt6Mlv8INobOccR8g"},
            {"hou", "orpEIUULaeYEkcdmf1sgEh8zkmnGlbQyKeBJzA_RlLacR4c0jMVo71KkTonslkmmMUQYk9DwhDXkWdCLF325-OE3_PJkgYmsYpib2XC4eZDWw_aqhHPpkONbX-P-NJ1WQZI9oKNqlWtjK_dMvKROi_pFkmDj86wgKCW1jQWaLpKFoXXbwEUPMzeMrFL35Odg_S8"},
            {"dal", "Xg2k-brl_yGLx1ewRVwG0bKzq_O-jmUlUcf8DdlRQ36S90guxhilTBt2BdXoKFHnUzxecJ0DbCt8ePS18P5SMWpqVelN-UA1OsSnw0TKMW2XkNlK0BQAuJfP5_rbSLoVt6kqnHRAYza0e8_6uRNeof4MpZ03aoHdFMcwXb0V4vetNvJS7BHY_PAXBI4Zv-pLaAs"},
            {"mty", "VxXQ_bhz-QPo9l2wR7-yxAZe76I9Ed5KeaoVCy8OZ_OKq_Kd3cjSWDs0fWBrLPXKsi57fHrOc8zn5W_tCAs7hBlBZ85y0KM_ZImLyOu0g4ZnFnYm4Y1YwoZpVaHRzE5AAssLr0iQcTSYcyiuuP7jVPFehskW7Wa6lHQ0X5nSjMI2a-seDZCWHEGxaOYU7n4_EfA"},
            {"gua", "lh6uOG2m5M5_9p_VJ1fYbESJzImV5TYThSiEXsD4xysiHENGXPz_Nw8lrXznJ8gLsv5DkhxsUdZTipHfJ2FijnJVKCrptkxecbuG-NZXKxhsB_V-Q3pXWxEqguQ6zKKHn3FqZj2IhCVwiuoXQpjZTeX_h-MxWmVNxf-vvaUdbrY0LLBG2v8zeQC5c7v0rc_Xkqk"},
        };

        public TrafficMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "traffic aus|hou|dal|mty|gau",
                Description = "traffic <placeCode>. Only works for Austin(aus), Houston(hou), Dallas(dal), Monterrey(mty), and Guadalajara(gua)."
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith("traffic");
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var placeCode = context.Message.FullText.Split(" ").Last();

            if (_places.TryGetValue(placeCode, out var mapData))
            {
                await context.SendResponse($"{_baseGoogleMapUrl}{mapData}");
                return;
            }

            await context.SendResponse($"Could not find data for {placeCode}. I only have data for: {string.Join(", ", _places.Select(x => x.Key))}");
        }
    }
}