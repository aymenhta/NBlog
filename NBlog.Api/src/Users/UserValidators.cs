using FluentValidation;

namespace NBlog.Api.Users;


public class FollowReqValidator : AbstractValidator<FollowReq>
{
	public FollowReqValidator()
	{
		RuleFor(x => x.UserName).NotEmpty();
		RuleFor(x => x.OtherUserName).NotEmpty();
	}
}
