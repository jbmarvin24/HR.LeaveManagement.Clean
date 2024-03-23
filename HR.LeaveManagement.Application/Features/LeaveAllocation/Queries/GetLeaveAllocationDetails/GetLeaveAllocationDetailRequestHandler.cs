using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;

public class GetLeaveAllocationDetailHandler :
    IRequestHandler<GetLeaveAllocationDetailQuery, LeaveAllocationDetailsDto>
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly IMapper _mapper;

    public GetLeaveAllocationDetailHandler(
        ILeaveAllocationRepository leaveAllocationRepository,
        IMapper mapper)
    {
        _leaveAllocationRepository=leaveAllocationRepository;
        _mapper=mapper;
    }
    public async Task<LeaveAllocationDetailsDto> Handle(
        GetLeaveAllocationDetailQuery request,
        CancellationToken cancellationToken)
    {
        var leaveAllocation = await _leaveAllocationRepository
            .GetLeaveAllocationWithDetails(request.Id);
        return _mapper.Map<LeaveAllocationDetailsDto>(leaveAllocation);
    }
}
