using ApiCatalog.Domain.Common;
using ApiCatalog.Domain.Validators;

namespace ApiCatalog.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(DomainMessages.Email.Required);

        var trimmed = value.Trim();
        
        return !RegexPatterns.Email().IsMatch(trimmed) 
            ? throw new ArgumentException(DomainMessages.Email.Invalid) 
                : new Email(trimmed.ToLowerInvariant());
    }   

    public override string ToString() => Value;
}
