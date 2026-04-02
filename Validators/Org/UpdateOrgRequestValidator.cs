using CrmWebApi.DTOs.Org;
using FluentValidation;

namespace CrmWebApi.Validators.Org;

public class UpdateOrgRequestValidator : AbstractValidator<UpdateOrgRequest>
{
    public UpdateOrgRequestValidator()
    {
        RuleFor(x => x.OrgTypeId).GreaterThan(0).When(x => x.OrgTypeId is not null);
        RuleFor(x => x.OrgName).MaximumLength(200).When(x => x.OrgName is not null);
        RuleFor(x => x.Inn).MaximumLength(12).When(x => x.Inn is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
    }
}
