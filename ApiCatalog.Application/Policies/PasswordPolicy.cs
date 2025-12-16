using ApiCatalog.Domain.Validators;

namespace ApiCatalog.Application.Policies;

public static class PasswordPolicy
{
    public static bool IsValid(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        if (password.Length < 6)
            return false;
        if (!RegexPatterns.PasswordHasLetter().IsMatch(password))
            return false;
        if (!RegexPatterns.PasswordHasDigit().IsMatch(password))
            return false;
        return true;
    }
}
