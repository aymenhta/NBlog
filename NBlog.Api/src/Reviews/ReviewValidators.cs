using FluentValidation;

namespace NBlog.Api.Reviews;


public class CreateReviewValidator : AbstractValidator<CreateReviewReq>
{
	public CreateReviewValidator()
	{
		RuleFor(x => x.Content).NotEmpty().MaximumLength(255);
		RuleFor(x => x.Value).InclusiveBetween(1, 10);
		RuleFor(x => x.PostId).GreaterThan(0);
	}
}
