namespace ApiCatalog.Domain.Errors;

public static class AuthErrors
{
    public const string UsernameRequired = "AUTH_USERNAME_REQUIRED";
    public const string PasswordRequired = "AUTH_PASSWORD_REQUIRED";
    public const string CpfRequired = "AUTH_CPF_REQUIRED";

    public const string InvalidEmail = "AUTH_INVALID_EMAIL";
    public const string WeakPassword = "AUTH_WEAK_PASSWORD";

    public const string UserAlreadyExists = "AUTH_USER_ALREADY_EXISTS";
    public const string EmailAlreadyExists = "AUTH_EMAIL_ALREADY_EXISTS";
    public const string CpfAlreadyExists = "AUTH_CPF_ALREADY_EXISTS";
}

