using System.Collections.Generic;
using Hsbot.Core.Messaging;
using SlackConnector.Models;

namespace Hsbot.Slack
{
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
