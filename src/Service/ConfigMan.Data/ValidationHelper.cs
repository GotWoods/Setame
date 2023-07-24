using System.Text.RegularExpressions;

namespace ConfigMan.Data;

public class ValidationHelper
{
    public static bool BeValidString(string name)
    {
        const string allowedPattern = @"^[\p{L}\p{N}\p{Zs}]+$";
        return !string.IsNullOrEmpty(name) && Regex.IsMatch(name, allowedPattern);
    }
}