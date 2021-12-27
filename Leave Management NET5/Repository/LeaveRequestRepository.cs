using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Leave_Management_NET5.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> findAll()
        {
            return _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .ToList();
        }

        public LeaveRequest FindById(int id)
        {
            return _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .FirstOrDefault(q => q.Id == id);
        }

        public List<LeaveRequest> GetLeaveRequestsByEmployee(string employeeId)
        {
            return findAll().Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveRequests.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0 ? true : false;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
