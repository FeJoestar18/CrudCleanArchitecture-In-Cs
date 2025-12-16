// csharp
namespace ApiCatalog.Domain.Common;

public static class DomainMessages
{
    public static class Cpf
    {
        public const string Required = "CPF é obrigatório.";
        public const string Invalid = "CPF inválido.";
    }

    public static class Email
    {
        public const string Required = "Email é obrigatório.";
        public const string Invalid = "Email inválido.";
    }
}