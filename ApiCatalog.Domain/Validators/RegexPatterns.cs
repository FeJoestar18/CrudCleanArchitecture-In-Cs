using System.Text.RegularExpressions;

namespace ApiCatalog.Domain.Validators;

public static partial class RegexPatterns
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public static partial Regex Email();

    [GeneratedRegex(@"\D")]
    public static partial Regex NonDigits();

    [GeneratedRegex(@"[A-Za-z]")]
    public static partial Regex PasswordHasLetter();

    [GeneratedRegex(@"\d")]
    public static partial Regex PasswordHasDigit();
}