namespace Hsbot.Core.Messaging.Formatting
{
    public static class DateFormat
    {
        //approximate format results commented with each option
        //note that chat clients may differ depending on implementation
        public const string DateNumeric = "{date_num}"; //YYYY-MM-DD
        public const string Date = "{date}"; //MMMM dd yyyy
        public const string DateLong = "{date_long}"; //dddd, MMMM dd yyyy
        public const string Time = "{time}"; //hh:mm tt
        public const string TimeLong = "{time_long}"; //hh:mm:ss tt
    }
}