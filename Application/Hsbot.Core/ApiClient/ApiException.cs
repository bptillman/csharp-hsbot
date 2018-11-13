namespace Hsbot.Core.ApiClient
{
    using System;

    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }
}
