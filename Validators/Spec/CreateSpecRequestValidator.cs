using CrmWebApi.DTOs.Spec;
using FluentValidation;

namespace CrmWebApi.Validators.Spec;

public class CreateSpecRequestValidator : AbstractValidator<CreateSpecRequest>
{
    public CreateSpecRequestValidator()
    {
        RuleFor(x => x.SpecName).NotEmpty().MaximumLength(200);
    }
}
