using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Setame.Data.Handlers;
using Setame.Web.Models;

namespace Setame.Web.Controllers
{
    public class ControllerHelper
    {
        public static IActionResult HttpResultFrom(CommandResponse response)
        {
            if (!response.WasSuccessful)
                return new BadRequestObjectResult(ErrorResponse.From(response));
            return new NoContentResult();
        }
    }
}
