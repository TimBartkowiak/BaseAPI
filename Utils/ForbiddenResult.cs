using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Utils
{
    public class ForbiddenResult : ObjectResult
    {
        public ForbiddenResult(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}