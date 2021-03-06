using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Leave_Management_NET5.Contracts;
using Leave_Management_NET5.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.AspNetCore.Identity;
using Leave_Management_NET5.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leave_Management_NET5.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        //private readonly ILeaveRequestRepository _leaveRequestRepository;
        //private readonly ILeaveTypeRepository _leaveTypeRepository;
        //private readonly ILeaveAllocationRepository leaveAllocationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Employee> userManager;
        private readonly IMapper _mapper;
        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepository,
            ILeaveTypeRepository leaveTypeRepository, UserManager<Employee> userManager,
            ILeaveAllocationRepository leaveAllocationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            //this._leaveRequestRepository = leaveRequestRepository;
            //this._leaveTypeRepository = leaveTypeRepository;
            this.userManager = userManager;
            //this.leaveAllocationRepository = leaveAllocationRepository;
            this._mapper = mapper;
            this._unitOfWork = unitOfWork;
        }

        // GET: LeaveRequestController
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            //var leaveRequest = await _leaveRequestRepository.findAll();
            var leaveRequest = await _unitOfWork.LeaveRequests.findAll(includes: q => q.Include(x => x.LeaveType).Include(x => x.RequestingEmployee));
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);

            foreach (var leaveRequestVM in leaveRequestModel)
            {
                leaveRequestVM.TotalDaysRequested = (int)(leaveRequestVM.EndDate - leaveRequestVM.StartDate).TotalDays + 1;
            }


            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestModel.Count,
                ApprovedRequests = leaveRequestModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequestModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequestModel.Count(q => q.Approved == false),
                leaveRequests = leaveRequestModel
            };

            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //var leaveRequest = await _leaveRequestRepository.FindById(id);
            var leaveRequest = await _unitOfWork.LeaveRequests.Find(q => q.Id == id, includes: q => q.Include(x => x.LeaveType).Include(x => x.RequestingEmployee).Include(x => x.ApprovedBy));
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);

            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var period = DateTime.Now.Year;
                var user = await userManager.GetUserAsync(User);
                //var leaveRequest = await _leaveRequestRepository.FindById(id);
                var leaveRequest = await _unitOfWork.LeaveRequests.Find(q => q.Id == id);

                var dayRequested = (int)(Convert.ToDateTime(leaveRequest.EndDate) - Convert.ToDateTime(leaveRequest.StartDate)).TotalDays;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var employeeId = leaveRequest.RequestingEmployeeId;

                //var allocation = await leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employeeId, leaveTypeId);
                var allocation = await _unitOfWork.LeaveAllocations.Find(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period);
                allocation.NumberOfDays -= dayRequested;

                leaveRequest.Approved = true;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = user.Id;

                //await _leaveRequestRepository.Update(leaveRequest);
                //await leaveAllocationRepository.Update(allocation);

                _unitOfWork.LeaveRequests.Update(leaveRequest);
                _unitOfWork.LeaveAllocations.Update(allocation);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                //var leaveRequest = await _leaveRequestRepository.FindById(id);
                var leaveRequest = await _unitOfWork.LeaveRequests.Find(q => q.Id == id);
                leaveRequest.Approved = false;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = user.Id;

                //var isSuccess = await _leaveRequestRepository.Update(leaveRequest);
                _unitOfWork.LeaveRequests.Update(leaveRequest);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> CancelRequest(int id)
        {
            // var leaveRequest = await _leaveRequestRepository.FindById(id);
            var leaveRequest = await _unitOfWork.LeaveRequests.Find(q => q.Id == id);
            leaveRequest.Cancelled = true;

            //            await _leaveRequestRepository.Update(leaveRequest);
            _unitOfWork.LeaveRequests.Update(leaveRequest);
            await _unitOfWork.Save();

            return RedirectToAction(nameof(MyLeave));
        }

        // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            var employee = await userManager.GetUserAsync(User);
            //var leaveTypes = await _leaveTypeRepository.GetLeaveTypesByEmployee(employee.Id);

            var leaveAllocation = await _unitOfWork.LeaveAllocations.findAll(q => q.EmployeeId == employee.Id);
            var leaveTypes = await _unitOfWork.LeaveTypes.findAll();

            var leaveTypeModel = leaveTypes.Join(leaveAllocation,
                                                    LT => LT.Id,
                                                    LA => LA.LeaveTypeId,
                                                    (LT, LA) => new LeaveType
                                                    {
                                                        Id = LT.Id,
                                                        Name = LT.Name,
                                                        DefaultDays = LT.DefaultDays,
                                                        DateCreated = LT.DateCreated

                                                    }).Distinct().ToList();

            var leaveTypeItems = leaveTypeModel.Select(q => new SelectListItem
            {

                Text = q.Name,
                Value = q.Id.ToString()

            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                //var leaveTypes = await _leaveTypeRepository.findAll();
                var leaveTypes = await _unitOfWork.LeaveTypes.findAll();
                var leaveTypeItem = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });

                model.LeaveTypes = leaveTypeItem;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);

                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start date sholud be less then the End date");
                    return View(model);
                }

                var period = DateTime.Now.Year;
                var employee = await userManager.GetUserAsync(User);
                //var allocation = await leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);
                var allocation = await _unitOfWork.LeaveAllocations.Find(q => q.EmployeeId == employee.Id && q.LeaveTypeId == model.LeaveTypeId && q.Period == period);

                int DaysRequested = (int)(endDate - startDate).TotalDays;

                if (DaysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "Requested leave days should be less then the allocated days");
                    View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    Cancelled = false,
                    RequestComments = model.RequestComments
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);

                //var isSuccess = await _leaveRequestRepository.Create(leaveRequest);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Error occurred while saving leave request.");
                //    View(model);
                //}
                await _unitOfWork.LeaveRequests.Create(leaveRequest);
                await _unitOfWork.Save();


                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {


                return View();
            }
        }

        public async Task<ActionResult> MyLeave()
        {
            var period = DateTime.Now.Year;
            var employee = userManager.GetUserAsync(User).Result;
            // var leaveAllocation = await leaveAllocationRepository.GetLeaveAllocationByEmployee(employee.Id);
            var leaveAllocation = await _unitOfWork.LeaveAllocations.findAll(q => q.EmployeeId == employee.Id && q.Period == period,
                                includes: q => q.Include(x => x.LeaveType));

            // var leaveRequest = await _leaveRequestRepository.GetLeaveRequestsByEmployee(employee.Id);
            var leaveRequest = await _unitOfWork.LeaveRequests.findAll(q => q.RequestingEmployeeId == employee.Id, includes: q => q.Include(x => x.LeaveType));

            var leaveRequestVMs = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);

            foreach (var leaveRequestVM in leaveRequestVMs)
            {
                leaveRequestVM.TotalDaysRequested = (int)(leaveRequestVM.EndDate - leaveRequestVM.StartDate).TotalDays + 1;
            }

            var model = new EmployeeLeaveRequestViewVM
            {
                leaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocation),
                leaveRequests = leaveRequestVMs
            };

            return View(model);
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
