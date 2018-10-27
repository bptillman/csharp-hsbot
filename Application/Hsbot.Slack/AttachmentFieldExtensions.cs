using System.Collections.Generic;
using Hsbot.Core.Messaging;
using SlackConnector.Models;

namespace Hsbot.Slack
{
    internal static class AttachmentFieldExtensions
    {
        public static IList<SlackAttachmentField> ToSlackAttachmentFields(this List<AttachmentField> attachmentFields)
        {
            var result = new List<SlackAttachmentField>();

            if (attachmentFields == null) return result;

            foreach (var attachmentField in attachmentFields)
            {
                result.Add(new SlackAttachmentField
                {
                    Title = attachmentField.Title,
                    Value = attachmentField.Value,
                    IsShort = attachmentField.IsShort
                });
            }

            return result;
        }
    }
}
