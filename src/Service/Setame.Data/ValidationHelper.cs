using System.Text.RegularExpressions;

namespace Setame.Data;

public class ValidationHelper
{
    public static bool BeValidString(string name)
    {
        const string allowedPattern = @"^[\p{L}\p{N}\p{Zs}\p{P}]*$";
        return Regex.IsMatch(name, allowedPattern);
    }
}