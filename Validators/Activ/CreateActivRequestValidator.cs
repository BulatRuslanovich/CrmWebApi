using CrmWebApi.DTOs.Activ;
using FluentValidation;

namespace CrmWebApi.Validators.Activ;

public class CreateActivRequestValidator : AbstractValidator<CreateActivRequest>
{
    public CreateActivRequestValidator()
    {
        RuleFor(x => x.OrgId).GreaterThan(0);
        RuleFor(x => x.StatusId).GreaterThan(0);
        RuleFor(x => x.DrugIds).NotNull();
    }
}
