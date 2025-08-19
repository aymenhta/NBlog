using FluentValidation;

namespace NBlog.Api.Users;


public class LoginValidator : AbstractValidator<LoginModel>
{
	public LoginValidator()
	{
		RuleFor(x => x.Username).NotEmpty();
		RuleFor(x => x.Password).NotEmpty();
	}
}


public class RegistrationValidator : AbstractValidator<RegistrationModel>
{
	public RegistrationValidator()
	{
		RuleFor(x => x.Username).NotEmpty();
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
		RuleFor(x => x.Password).NotEmpty();
	}
}
