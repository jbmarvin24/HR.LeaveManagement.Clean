using FluentValidation;
using HR.LeaveManagement.Application.Contracts.Persistence;

namespace HR.LeaveManagement.Application.Features.LeaveType.Commands.UpdateLeaveType;

public class UpdateLeaveTypeCommandValidator : 
    AbstractValidator<UpdateLeaveTypeCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public UpdateLeaveTypeCommandValidator(ILeaveTypeRepository leaveTypeRepository)
    {
        RuleFor(p => p.Id)
            .NotNull()
            .MustAsync(LeaveTypeMustExist);

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(70)
            .WithMessage("{PropertyName} must be fewer " +
            "than 70 characters.");

        RuleFor(p => p.DefaultDays)
            .LessThan(100)
                .WithMessage("{PropertyName} cannot exceed 100")
            .GreaterThan(1)
                .WithMessage("{PropertyName} cannot be less than 1");

        RuleFor(p => p)
            .MustAsync(LeaveTypeNameUnique)
            .WithName("Leave type already exists.");

        _leaveTypeRepository=leaveTypeRepository;
    }

    private async Task<bool> LeaveTypeMustExist(int id, CancellationToken token)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
        return leaveType is not null;
    }

    private Task<bool> LeaveTypeNameUnique(
        UpdateLeaveTypeCommand command,
        CancellationToken token)
        => _leaveTypeRepository.IsLeaveTypeUnique(command.Name);
}
