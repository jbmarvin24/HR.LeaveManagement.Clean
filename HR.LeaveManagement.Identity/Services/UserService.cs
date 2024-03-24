using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HR.LeaveManagement.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor)
    {
        _userManager=userManager;
        _contextAccessor=contextAccessor;
    }

    public string UserId { get => _contextAccessor.HttpContext?
            .User?.FindFirstValue("uid"); }

    public async Task<Employee> GetEmployee(string userId)
    {
        var employee = await _userManager.FindByIdAsync(userId);
        return new Employee
        {
            Id = employee.Id,
            Email = employee.Email,
            Firstname = employee.FirstName,
            Lastname = employee.LastName,
        };
    }

    public async Task<List<Employee>> GetEmployees()
    {
        var employees = await _userManager
            .GetUsersInRoleAsync("Employee");

        return employees.Select(employee => new Employee
        {
            Id = employee.Id,
            Email = employee.Email,
            Firstname = employee.FirstName,
            Lastname = employee.LastName,
        }).ToList();
    }
}
