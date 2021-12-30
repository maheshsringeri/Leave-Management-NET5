using System;
using System.Threading.Tasks;
using Leave_Management_NET5.Data;

namespace Leave_Management_NET5.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<LeaveType> LeaveTypes { get; }
        IGenericRepository<LeaveRequest> LeaveRequests { get; }
        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save();

    }
}
