using ApiCatalog.Domain.Common;
using ApiCatalog.Domain.Validators;

namespace ApiCatalog.Domain.ValueObjects;

public sealed class Cpf
{
    public string Value { get; }

    private Cpf(string value)
    {
        Value = value;
    }
    
    public static Cpf Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(DomainMessages.Cpf.Required);

        var digits = RegexPatterns.NonDigits().Replace(value, "");

        return digits.Length != 11 
            ? throw new ArgumentException(DomainMessages.Cpf.Invalid) 
                : new Cpf(digits);
    }

    public override string ToString() => Value;
}