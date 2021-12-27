using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leave_Management_NET5.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;

        public LeaveTypeRepository(ApplicationDbContext db, ILeaveAllocationRepository leaveAllocationRepository)
        {
            _db = db;
            this.leaveAllocationRepository = leaveAllocationRepository;
        }

        public bool Create(LeaveType entity)
        {
            _db.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            return Save();
        }

        public ICollection<LeaveType> findAll()
        {
            return _db.LeaveTypes.ToList();
        }

        public LeaveType FindById(int id)
        {
            return _db.LeaveTypes.Find(id);
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public List<LeaveType> GetLeaveTypesByEmployee(string employeeId)
        {
            var leaveAllocations = leaveAllocationRepository.findAll().Where(q => q.EmployeeId == employeeId).ToList();

            var leaveTypes = findAll().Join(leaveAllocations,
                                                LT => LT.Id,
                                                LA => LA.LeaveTypeId,
                                                (LT, LA) => new LeaveType
                                                {
                                                    Id = LT.Id,
                                                    Name = LT.Name,
                                                    DefaultDays = LT.DefaultDays,
                                                    DateCreated = LT.DateCreated
                                                }).Distinct().ToList();

            return leaveTypes;
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveTypes.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0 ? true : false;
        }

        public bool Update(LeaveType entity)
        {
            _db.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
