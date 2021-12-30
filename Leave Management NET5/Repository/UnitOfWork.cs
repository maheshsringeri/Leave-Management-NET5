using System;
using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System.Threading.Tasks;

namespace Leave_Management_NET5.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        ApplicationDbContext _context;

        private IGenericRepository<LeaveType> _LeaveTypes;
        private IGenericRepository<LeaveAllocation> _LeaveAllocations;
        private IGenericRepository<LeaveRequest> _LeaveRequests;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<LeaveType> LeaveTypes
        {
            get => _LeaveTypes ??= new GenericRepository<LeaveType>(_context);
        }
        public IGenericRepository<LeaveRequest> LeaveRequests
        {
            get => _LeaveRequests ??= new GenericRepository<LeaveRequest>(_context);
        }
        public IGenericRepository<LeaveAllocation> LeaveAllocations
        {
            get => _LeaveAllocations ??= new GenericRepository<LeaveAllocation>(_context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                _context.Dispose();
            }

        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
