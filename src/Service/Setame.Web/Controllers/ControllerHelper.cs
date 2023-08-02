using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Handlers;

namespace Setame.Web.Controllers
{
    public class ControllerHelper
    {
        public static IActionResult HttpResultFrom(CommandResponse response)
        {
            if (!response.WasSuccessful)
                return new BadRequestObjectResult(response);
            return new NoContentResult();
        }
    }
}
