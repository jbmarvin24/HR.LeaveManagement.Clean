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
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;

    public CancelLeaveRequestCommandHandler(
        ILeaveRequestRepository leaveRequestRepository,
        IEmailSender emailSender,
        ILeaveAllocationRepository leaveAllocationRepository)
    {
        _leaveRequestRepository=leaveRequestRepository;
        _emailSender=emailSender;
        _leaveAllocationRepository=leaveAllocationRepository;
    }
    public async Task<Unit> Handle(
        CancelLeaveRequestCommand request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (leaveRequest is null)
            throw new NotFoundException(nameof(LeaveRequest), request.Id);

        leaveRequest.Cancelled = true;
        await _leaveRequestRepository.UpdateAsync(leaveRequest);

        // if already approved, re-evaluate the employee's allocations for the leave type
        if (leaveRequest.Approved == true)
        {
            int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
            var allocation = await _leaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
            allocation.NumberOfDays += daysRequested;

            await _leaveAllocationRepository.UpdateAsync(allocation);
        }

        try
        {
            var email = new EmailMessage
            {
                To = string.Empty,
                Body = $"Your leave request for {leaveRequest.StartDate:D} to " +
            $"{leaveRequest.EndDate:D} has been cancelled successfully.",
                Subject = "Leave Request Cancelled"
            };

            await _emailSender.SendEmail(email);
        }
        catch (Exception)
        {
            // log error
        }

        return Unit.Value;
    }
}
