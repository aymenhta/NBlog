using NBlog.Api.Dtos;

namespace NBlog.Api.Repository;

public interface IAuthService
{
    Task<(int, string)> Registration(RegistrationModel model, string role);
    Task<(int, string)> Login(LoginModel model);
}