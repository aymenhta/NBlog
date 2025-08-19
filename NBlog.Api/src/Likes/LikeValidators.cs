using FluentValidation;

namespace NBlog.Api.Likes;


public class ReactReqValidator : AbstractValidator<ReactReq>
{
    public ReactReqValidator()
    {
        RuleFor(x => x.PostId).GreaterThan(0);
        RuleFor(x => x.Action).IsInEnum();
    }
} 
