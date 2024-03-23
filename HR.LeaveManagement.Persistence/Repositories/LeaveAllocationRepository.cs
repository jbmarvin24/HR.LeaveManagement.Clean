﻿using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories;

public class LeaveAllocationRepository :
    GenericRepository<LeaveAllocation>, ILeaveAllocationRepository
{
    public LeaveAllocationRepository(HrDatabaseContext context) : base(context)
    {
    }

    public async Task AddAllocations(List<LeaveAllocation> allocations)
    {
        await _context.AddRangeAsync(allocations);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AllocationExists(string userId, int leaveTypeId
        , int period)
    {
        return await _context.LeaveAllocations.AsNoTracking()
            .AnyAsync(col => col.EmployeeId == userId &&
             col.LeaveTypeId == leaveTypeId &&
             col.Period == period);
    }

    public async Task<List<LeaveAllocation>> GetLeaveAllocationsWithDetails()
    {
        return await _context.LeaveAllocations
            .Include(table => table.LeaveType)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<LeaveAllocation>> GetLeaveAllocationsWithDetails(
        string userId)
    {
        return await _context.LeaveAllocations
            .Where(col => col.EmployeeId == userId)
            .Include(table => table.LeaveType)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<LeaveAllocation?> GetLeaveAllocationWithDetails(int id)
    {
        return await _context.LeaveAllocations
            .Include(table => table.LeaveType)
            .AsNoTracking()
            .FirstOrDefaultAsync(col => col.Id == id);
    }

    public async Task<LeaveAllocation?> GetUserAllocations(string userId, int leaveTypeId)
    {
        return await _context.LeaveAllocations.AsNoTracking()
            .FirstOrDefaultAsync(col => col.EmployeeId == userId
            && col.LeaveTypeId == leaveTypeId);
    }
}