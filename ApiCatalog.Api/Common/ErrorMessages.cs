using System.Collections.Generic;

namespace ApiCatalog.Api.Common;

public static class ErrorMessages
{
    private static readonly Dictionary<string, string> Messages = new()
    {
        ["AUTH_USERNAME_REQUIRED"] = "Username obrigatório.",
        ["AUTH_PASSWORD_REQUIRED"] = "Senha obrigatória.",
        ["AUTH_CPF_REQUIRED"] = "CPF obrigatório.",
        ["AUTH_INVALID_EMAIL"] = "Email inválido.",
        ["AUTH_WEAK_PASSWORD"] = "Senha fraca. Use ao menos 6 caracteres, com letras e números.",
        ["AUTH_USER_ALREADY_EXISTS"] = "Usuário já existe.",
        ["AUTH_EMAIL_ALREADY_EXISTS"] = "Email já cadastrado.",
        ["AUTH_CPF_ALREADY_EXISTS"] = "CPF já cadastrado."
    };

    public static string Get(string code)
        => Messages.TryGetValue(code, out var msg)
            ? msg
            : "Erro desconhecido";
}

