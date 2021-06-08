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
    public class OrderUtility
    {
        private readonly DBContext _dbContext;

        public OrderUtility(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveReOccurenceOrder(int orderId, OrderMaster order, List<OrderDetailViewModel> orderItemList, OrderAssignment orderAssignment, DateTime shipdate)
        {
            var checkOrderNo = _dbContext.tbl_OrderMaster.Max(m => m.OrderNo) + 1;
            var org_order = order;
            order.ReOccurenceParentOrderId = orderId;
            order.ReOccurence = 0;
            order.ReOccurenceCycle = null;
            order.ReOccurenceStartDate = null;
            order.ReOccurenceFrequency = null;
            order.ReOccurenceStartDate = null;
            order.ReOccurenceWeekday = null;
            order.OrderId = 0;
            order.OrderNo = checkOrderNo;
            order.ShipDate = shipdate;

            if (order.ShipStartDate != null)
            {

                order.ShipStartDate = Convert.ToDateTime(shipdate.ToString("MM/dd/yyyy") + " " + order.ShipStartDate.Value.ToString("HH:mm"));
                order.ShipDate = Convert.ToDateTime(shipdate.ToString("MM/dd/yyyy") + " " + order.ShipDate.Value.ToString("HH:mm"));
            }
            else
            {
                var curDayName = shipdate.DayOfWeek.ToString();
                var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                if (checkCalenderDate1 != null)
                {
                    order.ShipStartDate = Convert.ToDateTime(shipdate.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                }
            }

            if (order.ShipEndDate != null)
            {
                try
                {
                    if (org_order.ShipEndDate.Value > org_order.ShipStartDate.Value)
                    {
                        order.ShipEndDate = order.ShipStartDate.Value.AddMinutes((org_order.ShipEndDate.Value - org_order.ShipStartDate.Value).Minutes);
                    }
                    else
                    {
                        order.ShipEndDate = order.ShipStartDate.Value.AddMinutes((org_order.ShipStartDate.Value - org_order.ShipEndDate.Value).Minutes);
                    }
                    
                }
                catch (Exception)
                {

                    order.ShipEndDate = null;
                }

            }
            else
            {
                order.ShipEndDate = order.ShipStartDate.Value.AddMinutes(30);
            }


            _dbContext.tbl_OrderMaster.Add(order);
            _dbContext.SaveChanges();

            foreach (var item in orderItemList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ItemId = item.ItemId,
                    Description = item.Description,
                    OrderId = order.OrderId,
                    PerUnitPrice = item.PerUnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    UnitId = item.UnitId,
                    IsActive = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                };
                _dbContext.tbl_OrderDetail.Add(orderDetail);
                _dbContext.SaveChanges();

            }


            if (orderAssignment != null)
            {
                orderAssignment.OrderId = order.OrderId;
                orderAssignment.OrderAssignmentId = 0;
                _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                _dbContext.SaveChanges();
            }

        }

        public void updateReOccurenceOrder(int orderId, OrderMaster order, List<OrderDetailViewModel> orderItemList, OrderAssignment orderAssignment, DateTime shipdate)
        {
            var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
            checkOrder.ShipId = order.ShipId;
            checkOrder.CustomerId = order.CustomerId;
            checkOrder.TotalAmount = order.TotalAmount;
            checkOrder.TaxAmount = order.TaxAmount;

            if (order.ShipStartDate != null)
            {
                checkOrder.ShipStartDate = Convert.ToDateTime(checkOrder.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + order.ShipStartDate.Value.ToString("HH:mm"));
                checkOrder.ShipDate = Convert.ToDateTime(checkOrder.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + order.ShipDate.Value.ToString("HH:mm"));
            }
            else
            {
                var curDayName = shipdate.DayOfWeek.ToString();
                var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                if (checkCalenderDate1 != null)
                {
                    checkOrder.ShipStartDate = Convert.ToDateTime(checkOrder.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                }
            }

            if (order.ShipEndDate != null)
            {
                try
                {
                    checkOrder.ShipEndDate = checkOrder.ShipStartDate.Value.AddMinutes((order.ShipEndDate.Value - order.ShipStartDate.Value).TotalMinutes);
                }
                catch (Exception)
                {

                    checkOrder.ShipEndDate = null;
                }

            }
            else
            {
                checkOrder.ShipEndDate = checkOrder.ShipStartDate.Value.AddMinutes(30);
            }

            _dbContext.SaveChanges();



            var checkOrderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == checkOrder.OrderId).FirstOrDefault();
            if (checkOrderAssignment != null)
            {
                if (orderAssignment != null)
                {
                    checkOrderAssignment.EmployeeId = orderAssignment.EmployeeId;
                    _dbContext.SaveChanges();
                }
                else
                {
                    _dbContext.tbl_OrderAssignment.Remove(checkOrderAssignment);
                    _dbContext.SaveChanges();

                }
            }
            else
            {
                if (orderAssignment != null)
                {
                    orderAssignment.OrderId = checkOrder.OrderId;
                    orderAssignment.OrderAssignmentId = 0;
                    _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                    _dbContext.SaveChanges();
                }
            }




        }
        public List<CustomerMasterViewModel> GetCustomerListWithShipAddress()
        {
            return ((from cust in _dbContext.tbl_CustomerMaster
                     join billing in _dbContext.tbl_CustomerBillings on cust.CustomerId equals billing.CustomerId
                     where cust.IsActive == 1
                     select new CustomerMasterViewModel
                     {
                         CustmoerId = billing.CustomerBillingId,
                         //CompanyName = cust.CompanyName + (ship1 != null ? (" (" + (ship1.Address1 ?? "") + " " + (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship1.Zip1 ?? "") + ")") : ""),
                         CompanyName = cust.CompanyName + (Convert.ToString(cust.CompanyName1).Trim() == "" ? "" : " (" + cust.CompanyName1 + ")") + " "+ (billing.Address1??"")+" "+ (billing.Address2 ?? "")
                     }
                     ).OrderBy(o=>o.CompanyName).ToList());

        }

    }
}



