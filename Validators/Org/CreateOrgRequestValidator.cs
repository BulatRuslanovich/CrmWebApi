using CrmWebApi.DTOs.Org;
using FluentValidation;

namespace CrmWebApi.Validators.Org;

public class CreateOrgRequestValidator : AbstractValidator<CreateOrgRequest>
{
    public CreateOrgRequestValidator()
    {
        RuleFor(x => x.OrgTypeId).GreaterThan(0);
        RuleFor(x => x.OrgName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Inn).MaximumLength(12).When(x => x.Inn is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
    }
}
