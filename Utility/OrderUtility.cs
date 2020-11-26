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

            order.ReOccurenceParentOrderId = orderId;
            order.OrderId = 0;
            order.OrderNo = checkOrderNo;
            order.ShipDate = shipdate;
            order.ShipStartDate = shipdate;

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


        public List<CustomerMasterViewModel> GetCustomerListWithShipAddress()
        {
            return ((from cust in _dbContext.tbl_CustomerMaster
                     //join ship in _dbContext.tbl_CustmoerShipping on cust.CustomerId equals ship.CustomerId
                     //into ship
                     //from ship1 in ship.DefaultIfEmpty()
                     //join city in _dbContext.tbl_Cities on ship1.CityId equals city.CityId
                     //into city
                     //from city1 in city.DefaultIfEmpty()
                     //join state in _dbContext.tbl_States on ship1.StateId equals state.StateId
                     //into state
                     //from state1 in state.DefaultIfEmpty()
                     where cust.IsActive == 1 
                     select new CustomerMasterViewModel
                     {
                         CustmoerId = cust.CustomerId,
                         //CompanyName = cust.CompanyName + (ship1 != null ? (" (" + (ship1.Address1 ?? "") + " " + (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship1.Zip1 ?? "") + ")") : ""),
                         CompanyName = cust.CompanyName,

                     }
                     ).ToList());

        }

    }
}



