using CrmWebApi.DTOs.Drug;
using FluentValidation;

namespace CrmWebApi.Validators.Drug;

public class UpdateDrugRequestValidator : AbstractValidator<UpdateDrugRequest>
{
    public UpdateDrugRequestValidator()
    {
        RuleFor(x => x.DrugName).MaximumLength(200).When(x => x.DrugName is not null);
        RuleFor(x => x.Brand).MaximumLength(200).When(x => x.Brand is not null);
        RuleFor(x => x.Form).MaximumLength(100).When(x => x.Form is not null);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}
