using CrmWebApi.DTOs.Phys;
using FluentValidation;

namespace CrmWebApi.Validators.Phys;

public class UpdatePhysRequestValidator : AbstractValidator<UpdatePhysRequest>
{
    public UpdatePhysRequestValidator()
    {
        RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName is not null);
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName is not null);
        RuleFor(x => x.MiddleName).MaximumLength(100).When(x => x.MiddleName is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => x.Email is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Position).MaximumLength(200).When(x => x.Position is not null);
    }
}
