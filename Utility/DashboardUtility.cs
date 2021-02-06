using Microsoft.Extensions.Options;
using FieldServiceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace FieldServiceApp.Utility
{
    public class DashboardUtility
    {
        private readonly DBContext _dbContext;

        public DashboardUtility(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DashboardOrderViewModel getOrderDetail(int id)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            try
            {
                model = (from order in _dbContext.tbl_OrderMaster
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                         into orderAssign
                         from orderAssign1 in orderAssign.DefaultIfEmpty()
                         join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                          into employee
                         from employee1 in employee.DefaultIfEmpty()
                         join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                         join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                            into city
                         from city1 in city.DefaultIfEmpty()
                         join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                                into state
                         from state1 in state.DefaultIfEmpty()

                         where order.OrderId == id
                         select new DashboardOrderViewModel
                         {
                             OrderId = order.OrderId,
                             ShipStartDate = order.ShipStartDate,
                             ShipEndDate = order.ShipEndDate,
                             CustomerName = customer.CompanyName,
                             EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                             EmployeeId = orderAssign1.EmployeeId,
                             CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + (shipping.Address2 ?? ""),
                             Color = employee1.Color ?? "rgb(228 211 91 / 63%)",
                             ReOccurenceParentOrderId = (order.ReOccurence ==1 ? 1 :(order.ReOccurenceParentOrderId ?? 0)) 
                         }).FirstOrDefault();

                var items = (from orderDetail in _dbContext.tbl_OrderDetail
                             join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                             where item.ItemId == orderDetail.ItemId && orderDetail.OrderId == model.OrderId && orderDetail.IsActive == 1
                             select new ItemMasterViewModel
                             {
                                 ItemCd = item.ItemCd,
                                 ItemDescription = item.ItemDescription
                             }).ToList();

                if (items.Count() > 0)
                {
                    model.ItemName = String.Join(',', items.Select(s => s.ItemCd).ToList());
                    model.ItemDescription = String.Join(',', items.Select(s => s.ItemDescription).ToList());
                }


                if (model != null)
                {
                    model.EmployeeList = _dbContext.tbl_EmployeeMaster
                                .Select(s => new EmployeeMasterViewModel
                                {
                                    EmployeeId = s.EmployeeId,
                                    FirstName = s.FirstName,
                                    LastName = s.LastName,
                                    MiddleName = s.MiddleName
                                })
                                .ToList();

                }


            }
            catch (Exception ex)
            {


            }

            return model;

        }


        public DashboardOrderViewModel getOrderDetailWithApartemt(int id)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            try
            {
                model = (from order in _dbContext.tbl_OrderMaster
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                         into orderAssign
                         from orderAssign1 in orderAssign.DefaultIfEmpty()
                         join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                          into employee
                         from employee1 in employee.DefaultIfEmpty()
                         join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                         join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                            into city
                         from city1 in city.DefaultIfEmpty()
                         join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                                into state
                         from state1 in state.DefaultIfEmpty()

                         where order.OrderId == id
                         select new DashboardOrderViewModel
                         {
                             OrderId = order.OrderId,
                             ShipStartDate = order.ShipStartDate,
                             ShipEndDate = order.ShipEndDate,
                             CustomerName = customer.CompanyName,
                             EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                             EmployeeId = orderAssign1.EmployeeId,
                             CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + (shipping.Address2 ?? ""),
                             Color = employee1.Color ?? "rgb(228 211 91 / 63%)",
                             Apartments = order.ApartmentIds,
                             ReOccurenceParentOrderId = (order.ReOccurence == 1 ? 1 : (order.ReOccurenceParentOrderId ?? 0))

                         }).FirstOrDefault();

                var items = (from orderDetail in _dbContext.tbl_OrderDetail
                             join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                             where item.ItemId == orderDetail.ItemId && orderDetail.OrderId == model.OrderId && orderDetail.IsActive == 1
                             select new ItemMasterViewModel
                             {
                                 ItemCd = item.ItemCd,
                                 ItemDescription = item.ItemDescription
                             }).ToList();

                if (!String.IsNullOrEmpty(model.Apartments))
                {

                    var apartmentList = model.Apartments.Split(',').ToList();
                    var iapartmentList = new List<int>();
                    foreach (var item in apartmentList)
                    {
                        iapartmentList.Add(Convert.ToInt32(item.Replace('/',' ').Trim()));
                    }

                    var apartmentnames = _dbContext.tbl_CustomerShippingApartments.Where(w => iapartmentList.Contains(w.ApartmentId)).Select(s => ((s.ApartmentNo??"") +" " + (s.ApartmentName??""))).ToList();
                    model.Apartments = String.Join(',', apartmentnames);
                }



                if (items.Count() > 0)
                {
                    model.ItemName = String.Join(',', items.Select(s => s.ItemCd).ToList());
                    model.ItemDescription = String.Join(',', items.Select(s => s.ItemDescription).ToList());
                }


                if (model != null)
                {
                    model.EmployeeList = _dbContext.tbl_EmployeeMaster
                                .Select(s => new EmployeeMasterViewModel
                                {
                                    EmployeeId = s.EmployeeId,
                                    FirstName = s.FirstName,
                                    LastName = s.LastName,
                                    MiddleName = s.MiddleName
                                })
                                .ToList();

                }


            }
            catch (Exception ex)
            {


            }

            return model;

        }

        public DashboardViewModel getDashBoardDetail(string _rolename, int _userId)
        {
            DashboardViewModel model = new DashboardViewModel();
            model.EmployeeList = _dbContext.tbl_EmployeeMaster
                               .Select(s => new EmployeeMasterViewModel
                               {
                                   EmployeeId = s.EmployeeId,
                                   FirstName = s.FirstName,
                                   LastName = s.LastName,
                                   MiddleName = s.MiddleName
                               })
                               .ToList();
            try
            {
                if (_rolename == "Admin")
                {
                    model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                       join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                       join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                       into orderAssign
                                       from orderAssign1 in orderAssign.DefaultIfEmpty()
                                       join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                        into employee
                                       from employee1 in employee.DefaultIfEmpty()
                                       join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                                       join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                        into city
                                       from city1 in city.DefaultIfEmpty()
                                       join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                       into state
                                       from state1 in state.DefaultIfEmpty()
                                       let items = (from orderDetail in _dbContext.tbl_OrderDetail
                                                    join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                                                    where item.ItemId == orderDetail.ItemId && orderDetail.OrderId == order.OrderId && order.IsActive == 1
                                                    select item.ItemCd).Distinct().ToList()
                                       where order.IsActive == 1
                                       select new OrderMasterViewModel
                                       {
                                           OrderNo = order.OrderNo,
                                           OrderId = order.OrderId,
                                           OrderDate = order.OrderDate,
                                           ShipStartDate = order.ShipStartDate,
                                           ShipEndDate = order.ShipEndDate,
                                           CustomerName = customer.CompanyName,
                                           EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                                           TotalAmount = order.TotalAmount,
                                           EmployeeId = orderAssign1.EmployeeId,
                                           ItemName = String.Join(',', items),
                                           //CustomerShipAddress = city1.CityName + " " + state1.StateName + " " + shipping.Address,
                                           CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + (shipping.Address2 ?? ""),
                                           Status = (orderAssign1 == null ? "New" : (orderAssign1.CompletedDate == null ? "Assigned" : "Completed")),
                                           Color = employee1.Color ?? "rgb(228 211 91 / 63%)",
                                           ReOccurenceParentOrderId = (order.ReOccurence == 1 ? 1 : (order.ReOccurenceParentOrderId ?? 0))
                                       })
                                               .ToList();
                }
                else
                {
                    var _employeeId = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == _userId).Select(s => s.EmployeeId).FirstOrDefault();
                    model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                       join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                       join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                       into orderAssign
                                       from orderAssign1 in orderAssign.DefaultIfEmpty()
                                       join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                        into employee
                                       from employee1 in employee.DefaultIfEmpty()
                                       join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                                       join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                        into city
                                       from city1 in city.DefaultIfEmpty()
                                       join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                       into state
                                       from state1 in state.DefaultIfEmpty()
                                       let items = (from orderDetail in _dbContext.tbl_OrderDetail
                                                    join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                                                    where item.ItemId == orderDetail.ItemId && orderDetail.OrderId == order.OrderId && order.IsActive == 1
                                                    select item.ItemCd).Distinct().ToList()
                                       where orderAssign1.EmployeeId == _employeeId && order.IsActive == 1
                                       select new OrderMasterViewModel
                                       {
                                           OrderNo = order.OrderNo,
                                           OrderId = order.OrderId,
                                           OrderDate = order.OrderDate,
                                           ShipStartDate = order.ShipStartDate,
                                           ShipEndDate = order.ShipEndDate,
                                           CustomerName = customer.CompanyName,
                                           EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                                           TotalAmount = order.TotalAmount,
                                           EmployeeId = orderAssign1.EmployeeId,
                                           ItemName = String.Join(',', items),
                                           //CustomerShipAddress = city1.CityName + " " + state1.StateName + " " + shipping.Address,
                                           CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + (shipping.Address2 ?? ""),
                                           Status = (orderAssign1 == null ? "New" : (orderAssign1.CompletedDate == null ? "Assigned" : "Completed")),
                                           Color = employee1.Color ?? "rgb(228 211 91 / 63%)",
                                           ReOccurenceParentOrderId = (order.ReOccurence == 1 ? 1 : (order.ReOccurenceParentOrderId ?? 0))
                                           // Color = employee.Color
                                       })
                                  .ToList();
                }


                var checkWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
                var checkHolidays = _dbContext.tbl_CalenderWorkingDays.Where(w => w.HolidayDate != null).Select(s => s.HolidayDate).ToList();

                model.OrderList = model.OrderList.OrderByDescending(o => o.OrderId).ToList();

                foreach (var order in model.OrderList)
                {
                    if (checkWeekOffs.Contains(order.ShipStartDate?.DayOfWeek.ToString()))
                    {
                        order.ScheduledOnNonWorkingDay = true;
                    }
                    else
                    {
                        var checkHoliday = checkHolidays.Where(w => w.Value.Day == order.ShipStartDate?.Day && w.Value.Month == order.ShipStartDate?.Month).Count();
                        if (checkHoliday > 0)
                        {
                            order.ScheduledOnNonWorkingDay = true;
                        }

                    }
                }

                return (model);


            }
            catch (Exception ex)
            {


            }

            return model;
        }


    }
}



