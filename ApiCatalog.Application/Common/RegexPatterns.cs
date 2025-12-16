namespace ApiCatalog.Application.Common;

public static class RegexPatterns
{
    public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public const string PasswordHasLetter = "[A-Za-z]";
    public const string PasswordHasDigit = "\\d";

    public const string NonDigits = "\\D";
}

