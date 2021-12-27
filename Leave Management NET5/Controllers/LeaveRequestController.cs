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

namespace Leave_Management_NET5.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly UserManager<Employee> userManager;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;
        private readonly IMapper _mapper;
        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepository,
            ILeaveTypeRepository leaveTypeRepository, UserManager<Employee> userManager,
            ILeaveAllocationRepository leaveAllocationRepository, IMapper mapper)
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._leaveTypeRepository = leaveTypeRepository;
            this.userManager = userManager;
            this.leaveAllocationRepository = leaveAllocationRepository;
            this._mapper = mapper;
        }

        // GET: LeaveRequestController
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveRequest = _leaveRequestRepository.findAll();
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
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequestRepository.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);

            return View(model);
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestRepository.FindById(id);

                var dayRequested = (int)(Convert.ToDateTime(leaveRequest.EndDate) - Convert.ToDateTime(leaveRequest.StartDate)).TotalDays;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var employeeId = leaveRequest.RequestingEmployeeId;
                var allocation = leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employeeId, leaveTypeId);
                allocation.NumberOfDays -= dayRequested;

                leaveRequest.Approved = true;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = user.Id;

                _leaveRequestRepository.Update(leaveRequest);
                leaveAllocationRepository.Update(allocation);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                var user = userManager.GetUserAsync(User).Result;

                var leaveRequest = _leaveRequestRepository.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = user.Id;

                var isSuccess = _leaveRequestRepository.Update(leaveRequest);

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult CancelRequest(int id)
        {
            var leaveRequest = _leaveRequestRepository.FindById(id);
            leaveRequest.Cancelled = true;

            _leaveRequestRepository.Update(leaveRequest);

            return RedirectToAction(nameof(MyLeave));
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var employee = userManager.GetUserAsync(User).Result;
            var leaveTypes = _leaveTypeRepository.GetLeaveTypesByEmployee(employee.Id);

            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
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
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var leaveTypes = _leaveTypeRepository.findAll();
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

                var employee = userManager.GetUserAsync(User).Result;
                var allocation = leaveAllocationRepository.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);

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

                var isSuccess = _leaveRequestRepository.Create(leaveRequest);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error occurred while saving leave request.");
                    View(model);
                }



                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {


                return View();
            }
        }

        public ActionResult MyLeave()
        {
            var employee = userManager.GetUserAsync(User).Result;
            var leaveAllocation = leaveAllocationRepository.GetLeaveAllocationByEmployee(employee.Id);
            var leaveRequest = _leaveRequestRepository.GetLeaveRequestsByEmployee(employee.Id);
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
