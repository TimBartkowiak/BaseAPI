using System;

namespace BaseAPI.Exceptions
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException() : base("Username/password incorrect")
        {
        }

        public UnAuthorizedException(string message) : base(message)
        {
        }
    }
}