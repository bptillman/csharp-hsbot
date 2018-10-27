using System.Collections.Generic;
using SlackConnector.Models;

namespace Hsbot.Core.Messaging
{
    public class AttachmentField
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool IsShort { get; set; }
    }

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
