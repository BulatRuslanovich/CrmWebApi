using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Spec;
using CrmWebApi.Repositories;

namespace CrmWebApi.Services.Impl;

public class SpecService(IGenericRepository<Spec> repo) : ISpecService
{
    public async Task<IEnumerable<SpecResponse>> GetAllAsync() =>
        (await repo.FindAsync(s => !s.IsDeleted)).Select(MapToResponse);

    public async Task<SpecResponse> GetByIdAsync(int id)
    {
        var spec = await repo.GetByIdAsync(id);
        if (spec is null || spec.IsDeleted)
            throw new KeyNotFoundException($"Специальность {id} не найдена");
        return MapToResponse(spec);
    }

    public async Task<SpecResponse> CreateAsync(CreateSpecRequest req)
    {
        var spec = new Spec { SpecName = req.SpecName};
        await repo.AddAsync(spec);
        return MapToResponse(spec);
    }

    public async Task DeleteAsync(int id)
    {
        var spec = await repo.GetByIdAsync(id);
        if (spec is null || spec.IsDeleted)
            throw new KeyNotFoundException($"Специальность {id} не найдена");
        spec.IsDeleted = true;
        await repo.UpdateAsync(spec);
    }

    private static SpecResponse MapToResponse(Spec s) => new(s.SpecId, s.SpecName);
}
