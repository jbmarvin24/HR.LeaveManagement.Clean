using FluentValidation;
using HR.LeaveManagement.Application.Contracts.Persistence;

namespace HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;

public class CreateLeaveTypeCommandValidator : 
    AbstractValidator<CreateLeaveTypeCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public CreateLeaveTypeCommandValidator(ILeaveTypeRepository leaveTypeRepository)
    {
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

    private Task<bool> LeaveTypeNameUnique(
        CreateLeaveTypeCommand command,
        CancellationToken token)
        => _leaveTypeRepository.IsLeaveTypeUnique(command.Name);
}
