using HR.LeaveManagement.Domain.Common;

namespace HR.LeaveManagement.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAsync();
    Task<T?> GetByIdAsync(int Id);
    Task CreateAsync(T Entity);
    Task UpdateAsync(T Entity);
    Task DeleteAsync(T Entity);
}
