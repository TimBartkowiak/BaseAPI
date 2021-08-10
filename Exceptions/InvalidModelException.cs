using System;

namespace BaseAPI.Exceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string description) : base(description) { }
    }
}