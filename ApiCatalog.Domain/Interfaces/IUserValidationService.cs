namespace ApiCatalog.Domain.Interfaces;

public interface IUserValidationService<RegisterDto>
{
    Task<Result> ValidateRegistrationAsync<Result>(RegisterDto dto);
}