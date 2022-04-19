using Microsoft.AspNetCore.Mvc;
using Sentry;
using System.Net;
using ZDC.Shared.Dtos;

namespace ZDC.Server.Extensions;

public static class SentryExtensions
{
    public static ActionResult ReturnActionResult(this SentryId id)
    {
        return new BadRequestObjectResult(new Response<string>
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "An error has occurred",
            Data = id.ToString()
        });
    }
}
