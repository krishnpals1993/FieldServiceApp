using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FieldServiceApp.Models;
using FieldServiceApp.Utility;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Infrastructure;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using FieldServiceApp.Filters;
using System.Linq.Dynamic;

namespace FieldServiceApp.Controllers
{
    [Authentication]
    public class CustomerController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IConfigurationProvider _mappingConfiguration;

        public CustomerController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IConfigurationProvider mappingConfiguration)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _mappingConfiguration = mappingConfiguration;
        }

        public IActionResult List()
        {


            return View(new CustomerMasterViewModel_datatable());
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody]JqueryDataTablesParameters param)
        {
            try
            {
                //return new JsonResult(new { error = "Internal Server Error" });

                HttpContext.Session.SetString(nameof(JqueryDataTablesParameters), System.Text.Json.JsonSerializer.Serialize(param));
                IQueryable<CustomerMasterViewModel_datatable> customerList;
                customerList = (from customer in _dbContext.tbl_CustomerMaster
                                select new CustomerMasterViewModel_datatable
                                {
                                    CustmoerId = customer.CustomerId,
                                    CompanyName = customer.CompanyName + (Convert.ToString(customer.CompanyName1).Trim()=="" ?"" : " ("+ customer.CompanyName1+")") ,
                                    CompanyType = customer.CompanyType,
                                    IsActive = customer.IsActive
                                });


                var size = customerList.Count();

                var customerBillingAll = new List<CustomerBillingViewModel>();
                var customerShippingAll = new List<CustmoerShippingViewModel>();
                if (Convert.ToString(param.Search?.Value) != "")
                {
                    var serchValue = param.Search?.Value.ToLower();
                    var customerFilterList = customerList.Where(w =>
                                  ((w.CompanyName ?? "").ToLower().Contains(serchValue) ? true :
                                  ((w.CompanyType ?? "").ToLower().Contains(serchValue) ? true : false)));

                     customerBillingAll = _dbContext.tbl_CustomerBillings.Where(w =>
                                   ((w.Address1 ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.Address2 ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.Address2 ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.FirstName ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.LastName ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.Zip1 ?? "").ToLower().Contains(serchValue) ? true :
                                   ((w.Zip2 ?? "").ToLower().Contains(serchValue) ? true : false))))))))
                                   .Select(s => new CustomerBillingViewModel
                                   {
                                       CustomerBillingId = s.CustomerBillingId,
                                       CustomerId = s.CustomerId


                                   })
                                   .ToList();
                    
                    var customerBilling = customerBillingAll.Select(s => s.CustomerId).Distinct().ToList();

                    var customerBillingList = customerList.Where(w => customerBilling.Contains(w.CustmoerId));

                     customerShippingAll = (from shipping in _dbContext.tbl_CustmoerShipping
                                               join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                               into city
                                               from city1 in city.DefaultIfEmpty()
                                               join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                               into state
                                               from state1 in state.DefaultIfEmpty()
                                               select new CustmoerShippingViewModel
                                               {
                                                   ShipId = shipping.CustomerShipId,
                                                   Address =   (shipping.Address ?? "") + (shipping.Address2 ?? "") + (shipping.Address2 ?? "") +" "+ (city1.CityName ?? "") + " " + (state1.StateName ?? ""),
                                                   Zip1 = shipping.Zip1,
                                                   Zip2 = shipping.Zip2,
                                                   CustmoerId = shipping.CustomerId,
                                                   CustomerBillingId = shipping.CustomerBillingId

                                               })
                                                  .Where(w =>
                                  ((w.Address ?? "").ToLower().Contains(serchValue) ? true :
                                  ((w.Zip1 ?? "").ToLower().Contains(serchValue) ? true :
                                  (w.Zip2 ?? "").ToLower().Contains(serchValue) ? true : false)))
                                                  .Select(s => new CustmoerShippingViewModel
                                                  {
                                                      ShipId = s.ShipId,
                                                      CustmoerId = s.CustmoerId,
                                                      CustomerBillingId = s.CustomerBillingId
                                                  }).ToList(); ;

                    var customerShipping = customerShippingAll.Select(s => s.CustmoerId).Distinct().ToList();

                    var customerShippingList = customerList.Where(w => customerShipping.Contains(w.CustmoerId));

                    customerList = customerFilterList.Union(customerBillingList).Union(customerShippingList);





                }


                if (param.Length == -1)
                {
                    var items = customerList
                                      .ProjectTo<CustomerMasterViewModel_datatable>(_mappingConfiguration)
                                      .ToArray();


                    var result = new JqueryDataTablesPagedResults<CustomerMasterViewModel_datatable>
                    {
                        Items = items,
                        TotalSize = size
                    };

                    return new JsonResult(new JqueryDataTablesResult<CustomerMasterViewModel_datatable>
                    {
                        Draw = param.Draw,
                        Data = result.Items,
                        RecordsFiltered = result.TotalSize,
                        RecordsTotal = result.TotalSize
                    });
                }
                else
                {

                    try
                    {
                        var orderbyColumn = param.Columns[param.Order[0].Column].Data ?? "CompanyName";
                        var orderbyDir = param.Order[0].Dir;
                        if (orderbyDir.ToString().ToUpper() == "ASC")
                        {
                            if (orderbyColumn == "CompanyName")
                            {
                                customerList = customerList.OrderBy(o => o.CompanyName);
                            }
                            else
                            {
                                customerList = customerList.OrderBy(o => o.CompanyType);
                            }

                        }
                        else
                        {
                            if (orderbyColumn == "CompanyName")
                            {
                                customerList = customerList.OrderByDescending(o => o.CompanyName);
                            }
                            else
                            {
                                customerList = customerList.OrderByDescending(o => o.CompanyType);
                            }

                        }
                    }
                    catch (Exception ex)
                    {


                    }

                    var items = customerList
                  .Skip((param.Start / param.Length) * param.Length)
                  .Take(param.Length)
                  .ProjectTo<CustomerMasterViewModel_datatable>(_mappingConfiguration)
                  .ToArray();




                    if (Convert.ToString(param.Search?.Value) != "")//Convert.ToString(param.Search?.Value) != ""
                    {
                        var customerIds = items.Select(s => s.CustmoerId).ToList();
                        var billingIds = customerBillingAll.Select(s => s.CustomerBillingId).ToList().Union(customerShippingAll.Select(s => s.CustomerBillingId).ToList()).ToList();
                        var shippings = customerShippingAll.Select(s => s.ShipId).ToList();
                        var customerBillings = _dbContext.tbl_CustomerBillings.Where(w => billingIds.Contains(w.CustomerBillingId)).ToList();
                        var customerShippings = (from shipping in _dbContext.tbl_CustmoerShipping
                                                 join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                                 into city
                                                 from city1 in city.DefaultIfEmpty()
                                                 join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                 into state
                                                 from state1 in state.DefaultIfEmpty()
                                                 where shippings.Contains(shipping.CustomerShipId)
                                                 select new CustmoerShippingViewModel_Datatable
                                                 {
                                                     CustomerBillingId = shipping.CustomerBillingId,
                                                     ShipId = shipping.CustomerShipId,
                                                     Address = shipping.Address ?? "",
                                                     Address2 = shipping.Address2 ?? "",
                                                     Address3 = shipping.Address3 ?? "",
                                                     Zip1 = shipping.Zip1 ?? "",
                                                     Zip2 = shipping.Zip2 ?? "",
                                                     CityName = city1.CityName ?? "",
                                                     StateName = state1.StateName ?? ""
                                                 }).ToList();


                        foreach (var customer in items)
                        {
                            customer.BillingDetail = customerBillings.Where(w => w.CustomerId == customer.CustmoerId)
                                              .Select(s => new CustomerBillingViewModel_Datatable
                                              {
                                                  CustomerBillingId = s.CustomerBillingId,
                                                  FirstName = s.FirstName ?? "",
                                                  LastName = s.LastName ?? "",
                                                  Address1 = s.Address1 ?? "",
                                                  Address2 = s.Address2 ?? "",
                                                  Address3 = s.Address3 ?? "",
                                                  Zip1 = s.Zip1 ?? "",
                                                  Zip2 = s.Zip2 ?? ""

                                              }).ToList();

                            foreach (var item in customer.BillingDetail)
                            {
                                item.Shippings = customerShippings.Where(w => w.CustomerBillingId == item.CustomerBillingId).ToList();

                            }
                        }

                    }
                    else
                    {
                        var customerIds = items.Select(s => s.CustmoerId).ToList();
                        var customerBillings = _dbContext.tbl_CustomerBillings.Where(w => customerIds.Contains(w.CustomerId)).ToList();
                        var customerShippings = (from shipping in _dbContext.tbl_CustmoerShipping
                                                 join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                                 into city
                                                 from city1 in city.DefaultIfEmpty()
                                                 join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                 into state
                                                 from state1 in state.DefaultIfEmpty()
                                                 where customerIds.Contains(shipping.CustomerId)
                                                 select new CustmoerShippingViewModel_Datatable
                                                 {
                                                     CustomerBillingId = shipping.CustomerBillingId,
                                                     Address = shipping.Address ?? "",
                                                     Address2 = shipping.Address2 ?? "",
                                                     Address3 = shipping.Address3 ?? "",
                                                     Zip1 = shipping.Zip1 ?? "",
                                                     Zip2 = shipping.Zip2 ?? "",
                                                     CityName = city1.CityName ?? "",
                                                     StateName = state1.StateName ?? "",
                                                     ShipId = shipping.CustomerShipId
                                                 }).ToList();


                        foreach (var customer in items)
                        {
                            customer.BillingDetail = customerBillings.Where(w => w.CustomerId == customer.CustmoerId)
                                              .Select(s => new CustomerBillingViewModel_Datatable
                                              {
                                                  CustomerBillingId = s.CustomerBillingId,
                                                  FirstName = s.FirstName ?? "",
                                                  LastName = s.LastName ?? "",
                                                  Address1 = s.Address1 ?? "",
                                                  Address2 = s.Address2 ?? "",
                                                  Address3 = s.Address3 ?? "",
                                                  Zip1 = s.Zip1 ?? "",
                                                  Zip2 = s.Zip2 ?? ""

                                              }).ToList();

                            foreach (var item in customer.BillingDetail)
                            {
                                item.Shippings = customerShippings.Where(w => w.CustomerBillingId == item.CustomerBillingId).ToList();

                            }
                        }

                    }

                    var result = new JqueryDataTablesPagedResults<CustomerMasterViewModel_datatable>
                    {
                        Items = items,
                        TotalSize = size
                    };

                    return new JsonResult(new JqueryDataTablesResult<CustomerMasterViewModel_datatable>
                    {
                        Draw = param.Draw,
                        Data = result.Items,
                        RecordsFiltered = result.TotalSize,
                        RecordsTotal = result.TotalSize
                    });
                }


            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

        public IActionResult Add()
        {
            CustomerMasterViewModel model = new CustomerMasterViewModel();
            try
            {
                model.CityList = _dbContext.tbl_Cities
                    .Where(w => w.IsActive == 1)
                            .Select(s => new CityViewModel
                            {
                                CityId = s.CityId,
                                CityName = s.CityName
                            })
                            .ToList();

                model.StateList = _dbContext.tbl_States
                    .Where(w => w.IsActive == 1)
                            .Select(s => new StateViewModel
                            {
                                StateId = s.StateId,
                                StateName = s.StateName
                            })
                            .ToList();
                model.Contacts.Add(new CustmoerContactViewModel());
                model.Shippings.Add(new CustmoerShippingViewModel());
                model.Shippings.FirstOrDefault().ApartmentList.Add(new CustomerShippingApartmentViewModel());

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CustomerMasterViewModel model)
        {
            var a = new Exception();
            try
            {


                if (ModelState.IsValid)
                {

                    var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CompanyName == model.CompanyName).FirstOrDefault();
                    if (checkCustomer != null)
                    {
                        ViewBag.ErrorMessage = "Customer name is already exists";
                        model.CityList = _dbContext.tbl_Cities
                           .Select(s => new CityViewModel
                           {
                               CityId = s.CityId,
                               CityName = s.CityName
                           })
                           .ToList();

                        model.StateList = _dbContext.tbl_States
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        return View(model);

                    }

                    CustomerMaster customerMaster = new CustomerMaster()
                    {
                        CompanyName = model.CompanyName,
                        //CityId = model.CityId == null ? 0 : model.CityId.Value,
                        //StateId = model.StateId == null ? 0 : model.StateId.Value,
                        //FirstName = model.FirstName,
                        //LastName = model.LastName,
                        //Address = model.Address,
                        //Address2 = model.Address2??"",
                        //Address3 = model.Address3??"",
                        CompanyType = model.CompanyType,
                        //Zip1 = model.Zip1,
                        //Zip2 = model.Zip2,
                        CompanyCode = model.Code,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.tbl_CustomerMaster.Add(customerMaster);
                    _dbContext.SaveChanges();

                    foreach (var item in model.Shippings)
                    {
                        if (Convert.ToString(item.Zip1 ?? "") != "" ||
                            Convert.ToString(item.Zip2 ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "" ||
                            Convert.ToString(item.StateId) != "0" ||
                            Convert.ToString(item.CityId) != "0" ||
                            Convert.ToString(item.Address ?? "") != "")
                        {
                            CustomerShipping custmoerShipping = new CustomerShipping()
                            {
                                CustomerId = customerMaster.CustomerId,
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CityId = item.CityId == null ? 0 : item.CityId.Value,
                                StateId = item.StateId == null ? 0 : item.StateId.Value,
                                Zip1 = item.Zip1,
                                Zip2 = item.Zip2,
                                Address = item.Address,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };
                            _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                            _dbContext.SaveChanges();


                            foreach (var apartment in item.ApartmentList)
                            {
                                if (Convert.ToString(apartment.ApartmentNo ?? "") != "" ||
                                Convert.ToString(apartment.ApartmentName ?? "") != "")
                                {

                                    try
                                    {
                                        var checkAprtment = _dbContext.tbl_CustomerShippingApartments.Where(w => w.CustomerShipId == custmoerShipping.CustomerShipId && w.ApartmentNo == apartment.ApartmentNo).FirstOrDefault();
                                        if (checkAprtment == null)
                                        {
                                            CustomerShippingApartment customerShippingApartment = new CustomerShippingApartment()
                                            {
                                                CustomerShipId = custmoerShipping.CustomerShipId,
                                                ApartmentNo = apartment.ApartmentNo,
                                                ApartmentName = apartment.ApartmentName,
                                                IsActive = 1,
                                                CreatedBy = 1,
                                                CreatedDate = DateTime.Now
                                            };
                                            _dbContext.tbl_CustomerShippingApartments.Add(customerShippingApartment);
                                            _dbContext.SaveChanges();
                                        }

                                    }
                                    catch (Exception wx)
                                    {

                                        a = wx;
                                    }


                                }


                            }


                        }
                    }

                    foreach (var item in model.Contacts)
                    {
                        if (Convert.ToString(item.FirstName ?? "") != "" ||
                            Convert.ToString(item.MiddleName ?? "") != "" ||
                            Convert.ToString(item.LastName ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "")
                        {
                            CustomerContact customerContactDetail = new CustomerContact()
                            {
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CustomerId = customerMaster.CustomerId,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };
                            _dbContext.tbl_CustmoerContact.Add(customerContactDetail);
                            _dbContext.SaveChanges();
                        }
                    }






                    ViewBag.SuccessMessage = "Customer added successfully";

                }
            }

            catch (Exception ex)
            {
                a = ex;
            }

            return View(model);
        }

        public IActionResult Edit(string id, string CustomerBillingId="", string ShipId="")
        {
            CustomerMasterViewModel model = new CustomerMasterViewModel();
            try
            {
                var checkIds = id.Split("-");
                var count = 0;
                foreach (var item in checkIds)
                {
                    if (count==0)
                    {
                        id = item;
                    }
                    if (count == 1)
                    {
                        CustomerBillingId = item;
                    }
                    if (count == 2)
                    {
                        ShipId = item;
                    }
                    count++;
                }

                model.CityList = _dbContext.tbl_Cities
                            .Select(s => new CityViewModel
                            {
                                CityId = s.CityId,
                                CityName = s.CityName
                            })
                            .ToList();

                model.StateList = _dbContext.tbl_States
                            .Select(s => new StateViewModel
                            {
                                StateId = s.StateId,
                                StateName = s.StateName
                            })
                            .ToList();
                int _customerId = 0;
                int.TryParse(id, out _customerId);

                var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustomerId == _customerId).FirstOrDefault();
                if (checkCustomer != null)
                {
                    model.CompanyName = checkCustomer.CompanyName;
                    model.CustmoerId = checkCustomer.CustomerId;
                    model.CompanyType = checkCustomer.CompanyType;
                    model.Notes = checkCustomer.Notes;
                    model.Code = checkCustomer.CompanyCode;
                    model.ShipId = 0;
                    model.CustomerBillingId = 0;
                    if (!String.IsNullOrEmpty(CustomerBillingId))
                    {
                        model.CustomerBillingId = Convert.ToInt32(CustomerBillingId);
                    }
                    if (!String.IsNullOrEmpty(ShipId))
                    {
                        model.ShipId = Convert.ToInt32(ShipId);
                    }

                    var checkCustomerContacts = _dbContext.tbl_CustmoerContact.Where(w => w.CustomerId == _customerId).ToList();

                    if (checkCustomerContacts.Count() > 0)
                    {
                        model.Contacts = checkCustomerContacts.Select(s => new CustmoerContactViewModel
                        {
                            CustmoerId = s.CustomerId,
                            FirstName = s.FirstName,
                            MiddleName = s.MiddleName,
                            LastName = s.LastName,
                            Email = s.Email,
                            Phone = s.Phone,
                            CustmoerContactId = s.CustomerContactId
                        }).ToList();
                    }
                    else
                    {
                        model.Contacts.Add(new CustmoerContactViewModel());
                    }

                    var checkCustomerBilling = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerId == _customerId).ToList();
                    var checkCusomerShippingList  = _dbContext.tbl_CustmoerShipping.Where(w => w.CustomerId == _customerId).ToList();
                    var checkShippingIds = checkCusomerShippingList.Select(s => s.CustomerShipId).ToList();
                    var checkCusomerShippingApartmentList = _dbContext.tbl_CustomerShippingApartments.Where(w => checkShippingIds.Contains(w.CustomerShipId) && w.IsActive == 1).ToList();
                    
                    if (checkCustomerBilling.Count() > 0)
                    {
                        
                        model.Billings = checkCustomerBilling.Select(s => new CustomerBillingViewModel
                        {
                            CustomerBillingId = s.CustomerBillingId,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            CustomerId = s.CustomerId,
                            CityId = s.CityId,
                            StateId = s.StateId,
                            Address1 = s.Address1,
                            Address2 = s.Address2,
                            Address3 = s.Address3,
                            Zip1 = s.Zip1,
                            Zip2 = s.Zip2,
                            Notes = s.Notes

                        }).ToList();

                        if (model.CustomerBillingId>0)
                        {
                            var billingAddressDetail = model.Billings.Where(w => w.CustomerBillingId == model.CustomerBillingId).FirstOrDefault();
                            model.Billings.Remove(model.Billings.Where(w => w.CustomerBillingId == model.CustomerBillingId).FirstOrDefault());
                            model.Billings.Insert(0, billingAddressDetail);
                            model.Billings = model.Billings.Take(28).ToList();
                         
                        }

                        foreach (var item in model.Billings)
                        {
                            
                            var checkCustomerShipping = checkCusomerShippingList.Where(w => w.CustomerBillingId == item.CustomerBillingId).ToList();

                            if (item.CustomerBillingId == model.CustomerBillingId)
                            {
                                if (model.ShipId>0)
                                {
                                    var shippingAddressDetail = checkCustomerShipping.Where(w => w.CustomerShipId == model.ShipId).FirstOrDefault();

                                    checkCustomerShipping.Remove(checkCustomerShipping.Where(w => w.CustomerShipId == model.ShipId).FirstOrDefault());
                                    checkCustomerShipping.Insert(0, shippingAddressDetail);
                                }
                               
                            }

                            if (checkCustomerShipping.Count() > 0)
                            {
                                item.Shippings = checkCustomerShipping.Select(s => new CustmoerShippingViewModel
                                {
                                    FirstName = s.FirstName,
                                    MiddleName = s.MiddleName,
                                    LastName = s.LastName,
                                    Email = s.Email,
                                    Phone = s.Phone,
                                    CustmoerId = s.CustomerId,
                                    CityId = s.CityId,
                                    StateId = s.StateId,
                                    Address = s.Address,
                                    Address2 = s.Address2,
                                    Address3 = s.Address3,
                                    ShipId = s.CustomerShipId,
                                    Zip1 = s.Zip1,
                                    Zip2 = s.Zip2,
                                    Notes = s.Notes

                                }).ToList();
                                foreach (var itemShip in item.Shippings)
                                {
                                    var checkApartment = checkCusomerShippingApartmentList.Where(w => w.CustomerShipId == itemShip.ShipId).ToList();
                                    if (checkApartment.Count() > 0)
                                    {
                                        itemShip.ApartmentList = checkApartment.Select(s => new CustomerShippingApartmentViewModel
                                        {
                                            ApartmentNo = s.ApartmentNo,
                                            ApartmentName = s.ApartmentName,
                                            ApartmentId = s.ApartmentId,
                                            Notes = s.Notes

                                        }).ToList();
                                    }
                                    else
                                    {
                                        itemShip.ApartmentList.Add(new CustomerShippingApartmentViewModel());
                                    }
                                }
                            }
                            else
                            {
                                item.Shippings.Add(new CustmoerShippingViewModel());
                                item.Shippings.FirstOrDefault().ApartmentList.Add(new CustomerShippingApartmentViewModel());
                            }
                        }
                    }
                    else
                    {
                        model.Billings.Add(new CustomerBillingViewModel());
                        model.Billings.FirstOrDefault().Shippings.Add(new CustmoerShippingViewModel());
                        model.Billings.FirstOrDefault().Shippings.FirstOrDefault().ApartmentList.Add(new CustomerShippingApartmentViewModel());
                    }

                }
            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CustomerMasterViewModel model)
        {
            try
            {
                if (true)
                {
                    var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CompanyName == model.CompanyName && w.CustomerId != model.CustmoerId).FirstOrDefault();
                    if (checkCustomer != null)
                    {
                        ViewBag.ErrorMessage = "Customer name is already exists";
                        model.CityList = _dbContext.tbl_Cities
                           .Select(s => new CityViewModel
                           {
                               CityId = s.CityId,
                               CityName = s.CityName
                           })
                           .ToList();

                        model.StateList = _dbContext.tbl_States
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        return View(model);

                    }

                    checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustomerId == model.CustmoerId).FirstOrDefault();

                    checkCustomer.CompanyName = model.CompanyName;
                    checkCustomer.CompanyCode = model.Code;
                    checkCustomer.ModifiedBy = 1;
                    checkCustomer.Notes = model.Notes;
                    checkCustomer.ModifiedDate = DateTime.Now;

                    _dbContext.SaveChanges();

                    var checkCustomerBillingAll = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerId == model.CustmoerId).ToList();
                    var checkCusomerShippingListAll = _dbContext.tbl_CustmoerShipping.Where(w => w.CustomerId == model.CustmoerId).ToList();
                    var checkShippingIds = checkCusomerShippingListAll.Select(s => s.CustomerShipId).ToList();
                    var checkCusomerShippingApartmentListAll = _dbContext.tbl_CustomerShippingApartments.Where(w => checkShippingIds.Contains(w.CustomerShipId)).ToList();


                    foreach (var item in model.Billings)
                    {
                        if (item.CustomerBillingId != 0)
                        {
                            var checkBilling = checkCustomerBillingAll.Where(w => w.CustomerBillingId == item.CustomerBillingId).FirstOrDefault();

                            checkBilling.FirstName = item.FirstName;
                            checkBilling.LastName = item.LastName;
                            checkBilling.CityId = item.CityId ?? 0;
                            checkBilling.StateId = item.StateId ?? 0;
                            checkBilling.Address1 = item.Address1;
                            checkBilling.Address2 = item.Address2;
                            checkBilling.Address3 = item.Address3;
                            checkBilling.Zip1 = item.Zip1;
                            checkBilling.Zip2 = item.Zip2;
                            checkBilling.Notes = item.Notes;
                            checkBilling.ModifiedBy = 1;
                            checkBilling.ModifiedDate = DateTime.Now;
                            _dbContext.SaveChanges();

                            foreach (var ship in item.Shippings)
                            {
                                if (ship.ShipId != 0)
                                {
                                    var checkShipping = checkCusomerShippingListAll.Where(w => w.CustomerShipId == ship.ShipId).FirstOrDefault();

                                    checkShipping.FirstName = ship.FirstName;
                                    checkShipping.LastName = ship.LastName;
                                    checkShipping.Email = ship.Email;
                                    checkShipping.Phone = ship.Phone;
                                    checkShipping.CityId = ship.CityId ?? 0;
                                    checkShipping.StateId = ship.StateId ?? 0;
                                    checkShipping.Address = ship.Address;
                                    checkShipping.Address2 = ship.Address2;
                                    checkShipping.Address3 = ship.Address3;
                                    checkShipping.Zip1 = ship.Zip1;
                                    checkShipping.Zip2 = ship.Zip2;
                                    checkShipping.Notes = ship.Notes;
                                    checkShipping.ModifiedBy = 1;
                                    checkShipping.ModifiedDate = DateTime.Now;
                                    _dbContext.SaveChanges();

                                    var checkPreviousApartments = checkCusomerShippingApartmentListAll.Where(w => w.CustomerShipId == ship.ShipId).ToList();
                                    foreach (var previousApartment in checkPreviousApartments)
                                    {
                                        previousApartment.IsActive = 0;
                                        previousApartment.ModifiedBy = 1;
                                        previousApartment.ModifiedDate = DateTime.Now;
                                      
                                    }
                                    _dbContext.SaveChanges();
                                    foreach (var apartment in ship.ApartmentList)
                                    {



                                        if (apartment.ApartmentId != 0)
                                        {
                                            var checkApartment = checkCusomerShippingApartmentListAll.Where(w => w.ApartmentId == apartment.ApartmentId).FirstOrDefault();

                                            checkApartment.ApartmentName = apartment.ApartmentName;
                                            checkApartment.ApartmentNo = apartment.ApartmentNo;
                                            checkApartment.Notes = apartment.Notes;
                                            checkApartment.IsActive = 1;
                                            checkApartment.ModifiedBy = 1;
                                            checkApartment.ModifiedDate = DateTime.Now;
                                            _dbContext.SaveChanges();
                                        }
                                        else
                                        {
                                            if (Convert.ToString(apartment.ApartmentNo ?? "") != "" ||
                                                Convert.ToString(apartment.ApartmentName ?? "") != "")
                                            {
                                                var checkAprtment = checkCusomerShippingApartmentListAll.Where(w => w.CustomerShipId == ship.ShipId && w.ApartmentNo == apartment.ApartmentNo).FirstOrDefault();
                                                if (checkAprtment == null)
                                                {
                                                    CustomerShippingApartment customerShippingApartment = new CustomerShippingApartment()
                                                    {
                                                        CustomerShipId = ship.ShipId,
                                                        ApartmentNo = apartment.ApartmentNo,
                                                        ApartmentName = apartment.ApartmentName,
                                                        Notes = apartment.Notes,
                                                        IsActive = 1,
                                                        CreatedBy = 1,
                                                        CreatedDate = DateTime.Now
                                                    };
                                                    _dbContext.tbl_CustomerShippingApartments.Add(customerShippingApartment);
                                                    _dbContext.SaveChanges();
                                                }

                                            }
                                        }




                                    }

                                }
                                else
                                {
                                    if (Convert.ToString(ship.Zip1 ?? "") != "" ||
                                    Convert.ToString(ship.Zip2 ?? "") != "" ||
                                    Convert.ToString(ship.Email ?? "") != "" ||
                                    Convert.ToString(ship.Phone ?? "") != "" ||
                                    Convert.ToString(ship.StateId) != "0" ||
                                    Convert.ToString(ship.CityId) != "0" ||
                                    Convert.ToString(ship.Address ?? "") != "")
                                    {
                                        CustomerShipping custmoerShipping = new CustomerShipping()
                                        {
                                            CustomerId = checkCustomer.CustomerId,
                                            CustomerBillingId = item.CustomerBillingId,
                                            FirstName = ship.FirstName,
                                            MiddleName = ship.MiddleName,
                                            LastName = ship.LastName,
                                            Email = ship.Email,
                                            Phone = ship.Phone,
                                            Zip1 = ship.Zip1,
                                            Zip2 = ship.Zip2,
                                            CityId = ship.CityId == null ? 0 : ship.CityId.Value,
                                            StateId = ship.StateId == null ? 0 : ship.StateId.Value,
                                            Address = ship.Address,
                                            Address2 = ship.Address2,
                                            Address3 = ship.Address3,
                                            Notes = ship.Notes,
                                            IsActive = 1,
                                            CreatedBy = 1,
                                            CreatedDate = DateTime.Now
                                        };
                                        _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                                        _dbContext.SaveChanges();

                                        foreach (var apartment in ship.ApartmentList)
                                        {
                                            if (Convert.ToString(apartment.ApartmentNo ?? "") != "" ||
                                                    Convert.ToString(apartment.ApartmentName ?? "") != "")
                                            {
                                                var checkAprtment = _dbContext.tbl_CustomerShippingApartments.Where(w => w.CustomerShipId == custmoerShipping.CustomerShipId && w.ApartmentNo == apartment.ApartmentNo).FirstOrDefault();
                                                if (checkAprtment == null)
                                                {
                                                    CustomerShippingApartment customerShippingApartment = new CustomerShippingApartment()
                                                    {
                                                        CustomerShipId = custmoerShipping.CustomerShipId,
                                                        ApartmentNo = apartment.ApartmentNo,
                                                        Notes = apartment.Notes,

                                                        ApartmentName = apartment.ApartmentName,
                                                        IsActive = 1,
                                                        CreatedBy = 1,
                                                        CreatedDate = DateTime.Now
                                                    };
                                                    _dbContext.tbl_CustomerShippingApartments.Add(customerShippingApartment);
                                                    _dbContext.SaveChanges();
                                                }


                                            }

                                        }

                                    }
                                }
                            }

                        }
                        else
                        {
                            if (Convert.ToString(item.Zip1 ?? "") != "" ||
                                    Convert.ToString(item.Zip2 ?? "") != "" ||
                                    Convert.ToString(item.FirstName ?? "") != "" ||
                                    Convert.ToString(item.LastName ?? "") != "" ||
                                    Convert.ToString(item.StateId) != "0" ||
                                    Convert.ToString(item.CityId) != "0" ||
                                    Convert.ToString(item.Address1 ?? "") != "")
                            {
                                CustomerBilling custmoerBilling = new CustomerBilling()
                                {
                                    CustomerId = checkCustomer.CustomerId,
                                    FirstName = item.FirstName,
                                    LastName = item.LastName,
                                    Notes = item.Notes,
                                    Zip1 = item.Zip1,
                                    Zip2 = item.Zip2,
                                    CityId = item.CityId ?? 0,
                                    StateId = item.StateId ?? 0,
                                    Address1 = item.Address1,
                                    Address2 = item.Address2,
                                    Address3 = item.Address3,
                                    IsActive = 1,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now
                                };
                                _dbContext.tbl_CustomerBillings.Add(custmoerBilling);
                                _dbContext.SaveChanges();
                                item.CustomerBillingId = custmoerBilling.CustomerBillingId;

                                foreach (var ship in item.Shippings)
                                {
                                    {
                                        if (Convert.ToString(ship.Zip1 ?? "") != "" ||
                                        Convert.ToString(ship.Zip2 ?? "") != "" ||
                                        Convert.ToString(ship.Email ?? "") != "" ||
                                        Convert.ToString(ship.Phone ?? "") != "" ||
                                        Convert.ToString(ship.StateId) != "0" ||
                                        Convert.ToString(ship.CityId) != "0" ||
                                        Convert.ToString(ship.Address ?? "") != "")
                                        {
                                            CustomerShipping custmoerShipping = new CustomerShipping()
                                            {
                                                CustomerId = checkCustomer.CustomerId,
                                                CustomerBillingId = item.CustomerBillingId,
                                                FirstName = ship.FirstName,
                                                MiddleName = ship.MiddleName,
                                                LastName = ship.LastName,
                                                Notes = ship.Notes,
                                                Email = ship.Email,
                                                Phone = ship.Phone,
                                                Zip1 = ship.Zip1,
                                                Zip2 = ship.Zip2,
                                                CityId = ship.CityId == null ? 0 : ship.CityId.Value,
                                                StateId = ship.StateId == null ? 0 : ship.StateId.Value,
                                                Address = ship.Address,
                                                Address2 = ship.Address2,
                                                Address3 = ship.Address3,
                                                IsActive = 1,
                                                CreatedBy = 1,
                                                CreatedDate = DateTime.Now
                                            };
                                            _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                                            _dbContext.SaveChanges();

                                            foreach (var apartment in ship.ApartmentList)
                                            {
                                                if (Convert.ToString(apartment.ApartmentNo ?? "") != "" ||
                                                        Convert.ToString(apartment.ApartmentName ?? "") != "")
                                                {
                                                    var checkAprtment = _dbContext.tbl_CustomerShippingApartments.Where(w => w.CustomerShipId == custmoerShipping.CustomerShipId && w.ApartmentNo == apartment.ApartmentNo).FirstOrDefault();
                                                    if (checkAprtment == null)
                                                    {
                                                        CustomerShippingApartment customerShippingApartment = new CustomerShippingApartment()
                                                        {
                                                            CustomerShipId = custmoerShipping.CustomerShipId,
                                                            ApartmentNo = apartment.ApartmentNo,
                                                            ApartmentName = apartment.ApartmentName,
                                                            Notes = apartment.Notes,
                                                            IsActive = 1,
                                                            CreatedBy = 1,
                                                            CreatedDate = DateTime.Now
                                                        };
                                                        _dbContext.tbl_CustomerShippingApartments.Add(customerShippingApartment);
                                                        _dbContext.SaveChanges();
                                                    }


                                                }

                                            }

                                        }
                                    }
                                }



                            }

                        }





                    }

                    foreach (var item in model.Contacts)
                    {
                        if (item.CustmoerContactId != 0)
                        {
                            var checkCustomerContact = _dbContext.tbl_CustmoerContact.Where(w => w.CustomerContactId == item.CustmoerContactId).FirstOrDefault();
                            checkCustomerContact.FirstName = item.FirstName;
                            checkCustomerContact.MiddleName = item.MiddleName;
                            checkCustomerContact.LastName = item.LastName;
                            checkCustomerContact.Email = item.Email;
                            checkCustomerContact.Phone = item.Phone;
                            checkCustomerContact.CustomerId = checkCustomer.CustomerId;
                            checkCustomerContact.ModifiedBy = 1;
                            checkCustomerContact.ModifiedDate = DateTime.Now;
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            if (Convert.ToString(item.FirstName ?? "") != "" ||
                           Convert.ToString(item.MiddleName ?? "") != "" ||
                           Convert.ToString(item.LastName ?? "") != "" ||
                           Convert.ToString(item.Email ?? "") != "" ||
                           Convert.ToString(item.Phone ?? "") != "")
                            {
                                CustomerContact customerContactDetail = new CustomerContact()
                                {
                                    FirstName = item.FirstName,
                                    MiddleName = item.MiddleName,
                                    LastName = item.LastName,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    CustomerId = checkCustomer.CustomerId,
                                    IsActive = 1,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now
                                };
                                _dbContext.tbl_CustmoerContact.Add(customerContactDetail);
                                _dbContext.SaveChanges();
                            }

                        }


                    }






                    ViewBag.SuccessMessage = "Customer updated successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }


        public IActionResult GetCustomerContact(int id)
        {
            var model = new CustmoerContactViewModel();
            ViewBag.index = id;
            return PartialView("_CustomerContact", model);

        }

        public IActionResult GetCustomerShipping(string id)
        {
            var model = new CustmoerShippingViewModel();
            ViewBag.index = id;
            var ids = id.Split("_");
            ViewBag.index = ids[0];
            ViewBag.parentIndex = ids[1];
            

            model.CityList = _dbContext.tbl_Cities
                               .Select(s => new CityViewModel
                               {
                                   CityId = s.CityId,
                                   CityName = s.CityName
                               })
                               .ToList();

            model.StateList = _dbContext.tbl_States
                        .Select(s => new StateViewModel
                        {
                            StateId = s.StateId,
                            StateName = s.StateName
                        })
                        .ToList();
            model.ApartmentList.Add(new CustomerShippingApartmentViewModel());
            return PartialView("_CustomerShipping", model);
        }

        public IActionResult GetCustomerShippingApartment(string id)
        {
            var model = new CustomerShippingApartmentViewModel();
            var ids = id.Split("_");
            ViewBag.parentIndex = ids[0];
            ViewBag.index = ids[1];
            ViewBag.mainParentIndex = ids[2];
            return PartialView("_CustomerShippingApartment", model);
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustomerId == id).FirstOrDefault();
                if (checkCustomer != null)
                {
                    checkCustomer.IsActive = 0;
                    checkCustomer.ModifiedBy = 1;
                    checkCustomer.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();
                }

                response.Status = "1";
                response.Message = "Customer deleted successfully";


            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }

            return Json(response);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}