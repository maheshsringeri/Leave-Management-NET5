using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Leave_Management_NET5.Contracts
{
    public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
    {
        Task<ICollection<LeaveType>> GetEmployeesByLeaveType(int id);

        Task<List<LeaveType>> GetLeaveTypesByEmployee(string employeeId);
    }

}
