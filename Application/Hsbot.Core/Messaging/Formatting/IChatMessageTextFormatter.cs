using System;

namespace Hsbot.Core.Messaging.Formatting
{
    /// <summary>
    /// Provides a common interface for chat-client-specific formatting on outgoing message text
    /// </summary>
    public interface IChatMessageTextFormatter
    {
        /// <summary>
        /// Formats a given date for the chat client
        /// </summary>
        /// <param name="date">The date to be formatted</param>
        /// <param name="format">A format string controlling the date display. NOTE: This string should use values from the <c>DateFormat</c> class, NOT the standard .NET format strings</param>
        /// <returns>The given date formatted for rendering by the chat client</returns>
        string FormatDate(DateTime date, string format);

        /// <summary>
        /// Formats a given user id for the chat client. For example: this method might translate a given user Id to a proper @ mention.
        /// </summary>
        /// <param name="userId">Id of the user to be formatted</param>
        /// <returns>The given user id formatted for rendering by the chat client</returns>
        string FormatUserMention(string userId);

        /// <summary>
        /// Formats a given channel for the chat client. For example: this method might translate a given channel name to a proper @ mention.
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns>The given channel name formatted for rendering by the chat client.</returns>
        string FormatChannelMention(string channelName);

        /// <summary>
        /// Formats a url for the chat client.
        /// </summary>
        /// <param name="url">Raw url to be formatted. NOTE: this should NOT be url encoded already.</param>
        /// <param name="displayText">Display text to be rendered in place of the url (if supported)</param>
        /// <returns>String representing a hyperlink formatted for rendering by the chat client.</returns>
        string FormatHyperlink(string url, string displayText);
    }
}
