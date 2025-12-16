using ApiCatalog.Application.Common;
using ApiCatalog.Application.DTOs;

namespace ApiCatalog.Application.Services.InterfaceService;

public interface IUserValidationService
{
    Task<ValidationResult> ValidateRegistrationAsync(RegisterDto dto);
}