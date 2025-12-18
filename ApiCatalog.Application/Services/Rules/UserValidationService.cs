using ApiCatalog.Application.Common;
using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Policies;
using ApiCatalog.Application.Services.Interface;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Domain.ValueObjects;

namespace ApiCatalog.Application.Services.Rules;

public class UserValidationService(IUserRepository userRepository, IRoleRepository roleRepository)
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
            Email email;
            try
            {
                email = Email.Create(dto.Email);
            }
            catch (ArgumentException ex)
            {
                return ValidationResult.Fail(ex.Message);
            }

            var byEmail = await userRepository.GetByEmailAsync(email.Value);
            if (byEmail != null)
                return ValidationResult.Fail(Messages.Auth.EmailAlreadyExists);
        }

        Cpf cpf;
        try
        {
            cpf = Cpf.Create(dto.Cpf);
        }
        catch (ArgumentException ex)
        {
            return ValidationResult.Fail(ex.Message);
        }

        var byCpf = await userRepository.GetByCpfAsync(cpf.Value);
        if (byCpf != null)
            return ValidationResult.Fail(Messages.Auth.CpfAlreadyExists);

        if (!PasswordPolicy.IsValid(dto.Password))
            return ValidationResult.Fail(Messages.Auth.WeakPassword);

        if (string.IsNullOrWhiteSpace(dto.Role))
            return ValidationResult.Success();

        var role = await roleRepository.GetByNameAsync(dto.Role);
        return role == null
            ? ValidationResult.Fail(Messages.Roles.RoleNotFound)
            : ValidationResult.Success();
    }
}
