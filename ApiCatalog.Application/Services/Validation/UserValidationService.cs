using System.Text.RegularExpressions;
using ApiCatalog.Application.Common;
using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services.InterfaceService;
using ApiCatalog.Domain.Interfaces;

namespace ApiCatalog.Application.Services.Validation;

public partial class UserValidationService(IUserRepository userRepository, IRoleRepository roleRepository)
    : IUserValidationService
{
    public async Task<ValidationResult> ValidateRegistrationAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            return ValidationResult.Fail(Messages.Auth.UsernameRequired);

        if (string.IsNullOrWhiteSpace(dto.Password))
            return ValidationResult.Fail(Messages.Auth.PasswordRequired);

        if (string.IsNullOrWhiteSpace(dto.Cpf))
            return ValidationResult.Fail(Messages.Auth.CpfRequired);

        var byUsername = await userRepository.GetByUsernameAsync(dto.Username);
        if (byUsername != null)
            return ValidationResult.Fail(Messages.Auth.UserAlreadyExists);

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (!Regex.IsMatch(dto.Email, RegexPatterns.Email))
                return ValidationResult.Fail(Messages.Auth.InvalidCpf); // small reuse: could add specific message

            var byEmail = await userRepository.GetByEmailAsync(dto.Email);
            if (byEmail != null)
                return ValidationResult.Fail(Messages.Auth.EmailAlreadyExists);
        }

        var byCpf = await userRepository.GetByCpfAsync(dto.Cpf);
        if (byCpf != null)
            return ValidationResult.Fail(Messages.Auth.CpfAlreadyExists);

        var cpfDigits = Regex.Replace(dto.Cpf, RegexPatterns.NonDigits, "");
        if (cpfDigits.Length != 11)
            return ValidationResult.Fail(Messages.Auth.InvalidCpf);

        if (dto.Password.Length < 6 ||
            !Regex.IsMatch(dto.Password, RegexPatterns.PasswordHasLetter) ||
            !Regex.IsMatch(dto.Password, RegexPatterns.PasswordHasDigit))
        {
            return ValidationResult.Fail(Messages.Auth.WeakPassword);
        }

        if (string.IsNullOrWhiteSpace(dto.Role)) return ValidationResult.Success();
        var role = await roleRepository.GetByNameAsync(dto.Role);
        
        return role == null ? ValidationResult.Fail(Messages.Roles.RoleNotFound) : ValidationResult.Success();
    }
}
