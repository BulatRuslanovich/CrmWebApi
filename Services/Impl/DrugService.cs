using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Drug;
using CrmWebApi.Repositories;

namespace CrmWebApi.Services.Impl;

public class DrugService(IGenericRepository<Drug> repo) : IDrugService
{
    public async Task<IEnumerable<DrugResponse>> GetAllAsync() =>
        (await repo.FindAsync(d => !d.IsDeleted)).Select(MapToResponse);

    public async Task<DrugResponse> GetByIdAsync(int id)
    {
        var drug = await repo.GetByIdAsync(id);
        if (drug is null || drug.IsDeleted)
            throw new KeyNotFoundException($"Препарат {id} не найден");
        return MapToResponse(drug);
    }

    public async Task<DrugResponse> CreateAsync(CreateDrugRequest req)
    {
        var drug = new Drug
        {
            DrugName        = req.DrugName,
            DrugBrand       = req.Brand,
            DrugForm        = req.Form,
            DrugDescription = req.Description
        };
        await repo.AddAsync(drug);
        return MapToResponse(drug);
    }

    public async Task<DrugResponse> UpdateAsync(int id, UpdateDrugRequest req)
    {
        var drug = await repo.GetByIdAsync(id);
        if (drug is null || drug.IsDeleted)
            throw new KeyNotFoundException($"Препарат {id} не найден");

        drug.DrugName        = req.DrugName        ?? drug.DrugName;
        drug.DrugBrand       = req.Brand           ?? drug.DrugBrand;
        drug.DrugForm        = req.Form            ?? drug.DrugForm;
        drug.DrugDescription = req.Description     ?? drug.DrugDescription;

        await repo.UpdateAsync(drug);
        return MapToResponse(drug);
    }

    public async Task DeleteAsync(int id)
    {
        var drug = await repo.GetByIdAsync(id);
        if (drug is null || drug.IsDeleted)
            throw new KeyNotFoundException($"Препарат {id} не найден");
        drug.IsDeleted = true;
        await repo.UpdateAsync(drug);
    }

    private static DrugResponse MapToResponse(Drug d) =>
        new(d.DrugId, d.DrugName, d.DrugBrand, d.DrugForm, d.DrugDescription);
}
