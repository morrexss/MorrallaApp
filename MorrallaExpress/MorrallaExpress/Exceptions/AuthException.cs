using System;
namespace MorrallaExpress.Exceptions
{
    public class AuthException : Exception
    {
        public string Content { get; }

        public AuthException()
        {
        }

        public AuthException(string content)
        {
            Content = content;
        }
    }
}
