using FluentValidation;

namespace NBlog.Api.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryReq>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(3, 34);
    }
}