using CrmWebApi.DTOs.Drug;
using FluentValidation;

namespace CrmWebApi.Validators.Drug;

public class CreateDrugRequestValidator : AbstractValidator<CreateDrugRequest>
{
    public CreateDrugRequestValidator()
    {
        RuleFor(x => x.DrugName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Brand).MaximumLength(200).When(x => x.Brand is not null);
        RuleFor(x => x.Form).MaximumLength(100).When(x => x.Form is not null);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}
