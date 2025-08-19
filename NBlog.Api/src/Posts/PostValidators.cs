using FluentValidation;

namespace NBlog.Api.Posts;


public class CreatePostValidator : AbstractValidator<CreatePostReq>
{
	public CreatePostValidator()
	{
		RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
		RuleFor(x => x.Content).NotEmpty().MaximumLength(600);
		RuleFor(x => x.CategoryId).GreaterThan(0);
	}
}
