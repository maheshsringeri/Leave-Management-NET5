using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;

namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        bool CheckAllocation(int leaveTypeId, string EmployeeId);
        ICollection<LeaveAllocation> GetLeaveAllocationByEmployee(string employeeId);

        LeaveAllocation GetLeaveAllocationByEmployeeAndType(string employeeId, int leaveTypeId);


    }
}
