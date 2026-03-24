using CrmWebApi.DTOs.Spec;

namespace CrmWebApi.Services;

public interface ISpecService
{
    Task<IEnumerable<SpecResponse>> GetAllAsync();
    Task<SpecResponse> GetByIdAsync(int id);
    Task<SpecResponse> CreateAsync(CreateSpecRequest req);
    Task DeleteAsync(int id);
}
