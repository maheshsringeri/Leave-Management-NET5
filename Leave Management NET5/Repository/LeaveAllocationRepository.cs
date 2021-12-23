using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Leave_Management_NET5.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;

            return findAll().Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period)
                            .Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> findAll()
        {
            return _db.LeaveAllocations.Include(q => q.LeaveType).ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _db.LeaveAllocations
                        .Include(q => q.Employee)
                        .Include(q => q.LeaveType)
                        .FirstOrDefault(q => q.Id == id);

        }

        public ICollection<LeaveAllocation> GetLeaveAllocationByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return findAll().Where(q => q.EmployeeId == id && q.Period == period).ToList();
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveAllocations.Any(q => q.Id == id);
            return exists;

        }

        public bool Save()
        {
            return _db.SaveChanges() > 0 ? true : false;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
