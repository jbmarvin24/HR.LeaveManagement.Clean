﻿using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Models.Email;
using HR.LeaveManagement.Domain;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;

public class CancelLeaveRequestCommandHandler :
    IRequestHandler<CancelLeaveRequestCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmailSender _emailSender;

    public CancelLeaveRequestCommandHandler(
        ILeaveRequestRepository leaveRequestRepository,
        IEmailSender emailSender)
    {
        _leaveRequestRepository=leaveRequestRepository;
        _emailSender=emailSender;
    }
    public async Task<Unit> Handle(
        CancelLeaveRequestCommand request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (leaveRequest is null)
            throw new NotFoundException(nameof(LeaveRequest), request.Id);

        leaveRequest.Cancelled = true;

        // Re-evalutate the employees allocation

        var email = new EmailMessage
        {
            To = string.Empty,
            Body = $"Your leave request for {leaveRequest.StartDate:D} to " +
            $"{leaveRequest.EndDate:D} has been cancelled successfully.",
            Subject = "Leave Request Cancelled"
        };

        await _emailSender.SendEmail(email);

        return Unit.Value;
    }
}
