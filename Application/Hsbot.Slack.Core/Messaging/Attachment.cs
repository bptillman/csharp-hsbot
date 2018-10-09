using System.Collections.Generic;
using SlackConnector.Models;

namespace Hsbot.Slack.Core.Messaging
{
    public class Attachment
    {
        public Attachment()
        {
            AttachmentFields = new List<AttachmentField>();
        }

        public string Text { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Fallback { get; set; }

        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }

        public string Color { get; set; }

        public List<AttachmentField> AttachmentFields { get; set; }

        public Attachment AddAttachmentField(string title, string value)
        {
            return AddAttachmentField(title, value, false);
        }

        public Attachment AddAttachmentField(string title, string value, bool isShort)
        {
            AttachmentFields.Add(new AttachmentField
            {
                Title = title,
                Value = value,
                IsShort = isShort
            });

            return this;
        }
    }

    internal static class AttachmentExtensions
    {
        public static IList<SlackAttachment> ToSlackAttachments(this List<Attachment> attachments)
        {
            var slackAttachments = new List<SlackAttachment>();

            if (attachments == null) return slackAttachments;

            foreach (var attachment in attachments)
            {
                slackAttachments.Add(new SlackAttachment
                {
                    Text = attachment.Text,
                    Title = attachment.Title,
                    Fallback = attachment.Fallback,
                    ImageUrl = attachment.ImageUrl,
                    ThumbUrl = attachment.ThumbUrl,
                    AuthorName = attachment.AuthorName,
                    ColorHex = attachment.Color,
                    Fields = attachment.AttachmentFields.ToSlackAttachmentFields()
                });
            }

            return slackAttachments;
        }
    }
}
