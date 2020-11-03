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

        public void SaveReOccurenceOrder(int orderId, OrderMaster order, OrderDetail orderDetail, OrderAssignment orderAssignment, DateTime shipdate)
        {
            var checkOrderNo = _dbContext.tbl_OrderMaster.Max(m => m.OrderNo) + 1;

            order.ReOccurenceParentOrderId = orderId;
            order.OrderId = 0;
            order.OrderNo = checkOrderNo;
            order.ShipDate = shipdate;
            order.ShipStartDate = shipdate;

            _dbContext.tbl_OrderMaster.Add(order);
            _dbContext.SaveChanges();

            if (orderDetail != null)
            {
                orderDetail.OrderDetailId = 0;
                orderDetail.OrderId = order.OrderId;
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


        public List<CustomerMasterViewModel> GetCustomerListWithShipAddress()
        {
            return ((from cust in _dbContext.tbl_CustomerMaster
                     join ship in _dbContext.tbl_CustmoerShipping on cust.CustmoerId equals ship.CustmoerId
                     into ship
                     from ship1 in ship.DefaultIfEmpty()
                     join city in _dbContext.tbl_Cities on ship1.CityId equals city.CityId
                     into city
                     from city1 in city.DefaultIfEmpty()
                     join state in _dbContext.tbl_States on ship1.StateId equals state.StateId
                     into state
                     from state1 in state.DefaultIfEmpty()
                     where cust.IsActive == 1
                     select new CustomerMasterViewModel
                     {
                         CustmoerId = cust.CustmoerId,
                         CompanyName = cust.CompanyName + (ship1 != null ? (" (" + (ship1.Address ?? "") + " " + (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship1.Zip1 ?? "") + ")") : ""),

                     }
                     ).ToList());

        }

    }
}



