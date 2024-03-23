using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain.Common;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly HrDatabaseContext _context;

    public GenericRepository(HrDatabaseContext context)
    {
        _context=context;
    }

    public async Task CreateAsync(T Entity)
    {
        await _context.AddAsync(Entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T Entity)
    {
        _context.Remove(Entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int Id)
    {
        return await _context.Set<T>().AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == Id);
    }

    public async Task UpdateAsync(T Entity)
    {
        _context.Entry(Entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
