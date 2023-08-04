using Setame.Data.Handlers;

namespace Setame.Web.Models;

public class ErrorResponse //used to map the object error to a string error
{
    public bool WasSuccessful { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ErrorResponse From(CommandResponse response)
    {
        var result = new ErrorResponse();
        result.WasSuccessful = response.WasSuccessful;
        foreach (var responseError in response.Errors)
        {
            result.Errors.Add(responseError.ToString());
        }

        return result;
    }
}