using FluentValidation;


namespace NBlog.Api.Categories;


public class CreateCommentValidator : AbstractValidator<CreateCommentReq>
{
	public CreateCommentValidator()
	{
		RuleFor(x => x.Content).NotEmpty().MaximumLength(200);
		RuleFor(x => x.PostId).GreaterThan(0);
	}
}
