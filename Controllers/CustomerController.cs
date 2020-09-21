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

namespace FieldServiceApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public CustomerController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
        }

        public IActionResult List()
        {
            List<CustomerMasterViewModel> CustomerList = (from customer in _dbContext.tbl_CustomerMaster
                                                          join city in _dbContext.tbl_Cities on customer.CityId equals city.CityId
                                                          into city
                                                          from city1 in city.DefaultIfEmpty()
                                                          join state in _dbContext.tbl_States on customer.StateId equals state.StateId
                                                          into state
                                                          from state1 in state.DefaultIfEmpty()
                                                          select new CustomerMasterViewModel
                                                          {
                                                              CustmoerId = customer.CustmoerId,
                                                              CompanyName = customer.CompanyName,
                                                              CityName = city1.CityName,
                                                              StateName = state1.StateName,
                                                              Address = customer.Address,
                                                              IsActive = customer.IsActive
                                                          })
                                                   .ToList();
            return View(CustomerList);
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
                        CityId = model.CityId == null ? 0 : model.CityId.Value,
                        StateId = model.StateId == null ? 0 : model.StateId.Value,
                        Address = model.Address,
                        CompanyType = model.CompanyType,
                        Zip1 = model.Zip1,
                        Zip2= model.Zip2,
                        Code = model.Code,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.tbl_CustomerMaster.Add(customerMaster);
                    _dbContext.SaveChanges();

                    foreach (var item in model.Shippings)
                    {
                        if (Convert.ToString(item.FirstName ?? "") != "" ||
                            Convert.ToString(item.MiddleName ?? "") != "" ||
                            Convert.ToString(item.LastName ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "" ||
                            Convert.ToString(item.StateId) != "0" ||
                            Convert.ToString(item.CityId) != "0" ||
                            Convert.ToString(item.Address ?? "") != "")
                        {
                            CustmoerShipping custmoerShipping = new CustmoerShipping()
                            {
                                CustmoerId = customerMaster.CustmoerId,
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CityId = item.CityId == null ? 0 : item.CityId.Value,
                                StateId = item.StateId == null ? 0 : item.StateId.Value,
                                Address = item.Address,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };
                            _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                            _dbContext.SaveChanges();

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
                            CustmoerContact customerContactDetail = new CustmoerContact()
                            {
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CustmoerId = customerMaster.CustmoerId,
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
                var a = "";
            }

            return View(model);
        }

        public IActionResult Edit(string id)
        {
            CustomerMasterViewModel model = new CustomerMasterViewModel();
            try
            {
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

                var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustmoerId == _customerId).FirstOrDefault();
                if (checkCustomer != null)
                {
                    model.CompanyName = checkCustomer.CompanyName;
                    model.Address = checkCustomer.Address;
                    model.CityId = checkCustomer.CityId;
                    model.StateId = checkCustomer.StateId;
                    model.CustmoerId = checkCustomer.CustmoerId;
                    model.Zip1 = checkCustomer.Zip1;
                    model.Zip2 = checkCustomer.Zip2;
                    model.CompanyType = checkCustomer.CompanyType;
                    model.Code = checkCustomer.Code;

                    var checkCustomerContacts = _dbContext.tbl_CustmoerContact.Where(w => w.CustmoerId == _customerId).ToList();

                    if (checkCustomerContacts.Count() > 0)
                    {
                        model.Contacts = checkCustomerContacts.Select(s => new CustmoerContactViewModel
                        {
                            CustmoerId = s.CustmoerId,
                            FirstName = s.FirstName,
                            MiddleName = s.MiddleName,
                            LastName = s.LastName,
                            Email = s.Email,
                            Phone = s.Phone,
                            CustmoerContactId = s.CustmoerContactId
                        }).ToList();
                    }
                    else
                    {
                        model.Contacts.Add(new CustmoerContactViewModel());
                    }

                    var checkCustomerShipping = _dbContext.tbl_CustmoerShipping.Where(w => w.CustmoerId == _customerId).ToList();

                    if (checkCustomerShipping.Count() > 0)
                    {
                        model.Shippings = checkCustomerShipping.Select(s => new CustmoerShippingViewModel
                        {
                            FirstName = s.FirstName,
                            MiddleName = s.MiddleName,
                            LastName = s.LastName,
                            Email = s.Email,
                            Phone = s.Phone,
                            CustmoerId = s.CustmoerId,
                            CityId = s.CityId,
                            StateId = s.StateId,
                            Address = s.Address,
                            ShipId = s.ShipId


                        }).ToList();
                    }
                    else
                    {
                        model.Shippings.Add(new CustmoerShippingViewModel());
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
                if (ModelState.IsValid)
                {
                    var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CompanyName == model.CompanyName && w.CustmoerId != model.CustmoerId).FirstOrDefault();
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

                    checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustmoerId == model.CustmoerId).FirstOrDefault();

                    checkCustomer.CompanyName = model.CompanyName;
                    checkCustomer.CityId = model.CityId == null ? 0 : model.CityId.Value;
                    checkCustomer.StateId = model.StateId == null ? 0 : model.StateId.Value;
                    checkCustomer.Address = model.Address;
                    checkCustomer.Zip1 = model.Zip1;
                    checkCustomer.Zip2 = model.Zip2;
                    checkCustomer.Code = model.Code;
                    checkCustomer.ModifiedBy = 1;
                    checkCustomer.ModifiedDate = DateTime.Now;

                    _dbContext.SaveChanges();

                    foreach (var item in model.Shippings)
                    {
                        if (item.ShipId != 0)
                        {
                            var checkShipping = _dbContext.tbl_CustmoerShipping.Where(w => w.ShipId == item.ShipId).FirstOrDefault();

                            checkShipping.FirstName = item.FirstName;
                            checkShipping.MiddleName = item.MiddleName;
                            checkShipping.LastName = item.LastName;
                            checkShipping.Email = item.Email;
                            checkShipping.Phone = item.Phone;
                            checkShipping.CityId = item.CityId == null ? 0 : item.CityId.Value;
                            checkShipping.StateId = item.StateId == null ? 0 : item.StateId.Value;
                            checkShipping.Address = item.Address;
                            checkShipping.ModifiedBy = 1;
                            checkShipping.ModifiedDate = DateTime.Now;
                            _dbContext.SaveChanges();

                        }
                        else
                        {
                            if (Convert.ToString(item.FirstName ?? "") != "" ||
                            Convert.ToString(item.MiddleName ?? "") != "" ||
                            Convert.ToString(item.LastName ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "" ||
                            Convert.ToString(item.StateId) != "0" ||
                            Convert.ToString(item.CityId) != "0" ||
                            Convert.ToString(item.Address ?? "") != "")
                            {
                                CustmoerShipping custmoerShipping = new CustmoerShipping()
                                {
                                    CustmoerId = checkCustomer.CustmoerId,
                                    FirstName = item.FirstName,
                                    MiddleName = item.MiddleName,
                                    LastName = item.LastName,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    CityId = item.CityId == null ? 0 : item.CityId.Value,
                                    StateId = item.StateId == null ? 0 : item.StateId.Value,
                                    Address = item.Address,
                                    IsActive = 1,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now
                                };
                                _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                                _dbContext.SaveChanges();

                            }
                        }


                    }

                    foreach (var item in model.Contacts)
                    {
                        if (item.CustmoerContactId != 0)
                        {
                            var checkCustomerContact = _dbContext.tbl_CustmoerContact.Where(w => w.CustmoerContactId == item.CustmoerContactId).FirstOrDefault();
                            checkCustomerContact.FirstName = item.FirstName;
                            checkCustomerContact.MiddleName = item.MiddleName;
                            checkCustomerContact.LastName = item.LastName;
                            checkCustomerContact.Email = item.Email;
                            checkCustomerContact.Phone = item.Phone;
                            checkCustomerContact.CustmoerId = checkCustomer.CustmoerId;
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
                                CustmoerContact customerContactDetail = new CustmoerContact()
                                {
                                    FirstName = item.FirstName,
                                    MiddleName = item.MiddleName,
                                    LastName = item.LastName,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    CustmoerId = checkCustomer.CustmoerId,
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

        public IActionResult GetCustomerShipping(int id)
        {
            var model = new CustmoerShippingViewModel();
            ViewBag.index = id;
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
            return PartialView("_CustomerShipping", model);
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CustmoerId == id).FirstOrDefault();
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



    }
}