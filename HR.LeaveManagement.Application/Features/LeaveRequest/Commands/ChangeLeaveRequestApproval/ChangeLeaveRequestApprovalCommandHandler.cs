using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Models.Email;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;

public class ChangeLeaveRequestApprovalCommandHandler :
    IRequestHandler<ChangeLeaveRequestApprovalCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;

    public ChangeLeaveRequestApprovalCommandHandler(
        ILeaveRequestRepository leaveRequestRepository,
        ILeaveTypeRepository leaveTypeRepository,
        IMapper mapper,
        IEmailSender emailSender,
        ILeaveAllocationRepository leaveAllocationRepository)

    {
        _leaveRequestRepository=leaveRequestRepository;
        _leaveTypeRepository=leaveTypeRepository;
        _mapper=mapper;
        _emailSender=emailSender;
        _leaveAllocationRepository=leaveAllocationRepository;
    }
    public async Task<Unit> Handle(
        ChangeLeaveRequestApprovalCommand request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (leaveRequest is null)
            throw new NotFoundException(nameof(LeaveRequest), request.Id);

        leaveRequest.Approved = request.Approved;
        await _leaveRequestRepository.UpdateAsync(leaveRequest);

        // if request is approved, get and update the employee allocations.
        if (request.Approved)
        {
            int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
            var allocation = await _leaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
            allocation.NumberOfDays -= daysRequested;

            await _leaveAllocationRepository.UpdateAsync(allocation);
        }

        var email = new EmailMessage
        {
            To = string.Empty,
            Body = $"Your leave request for {leaveRequest.StartDate:D} to " +
            $"{leaveRequest.EndDate:D} has been updated.",
            Subject = "Leave Request Approval Status Updated"
        };

        await _emailSender.SendEmail(email);

        return Unit.Value;
    }
}
