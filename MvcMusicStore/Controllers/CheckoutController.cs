using Microsoft.AspNetCore.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using MvcMusicStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace MvcMusicStore.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddressPayment()
        {
            var order = new Order();
            ShoppingCartRepo SCRepo = new ShoppingCartRepo();

            SCRepo.ConfirmIndicatorFromCheckout(this.HttpContext);
            string indicator = SCRepo.CheckIfLoggedOn(this.HttpContext);

            if (indicator != "True")
            {
                //SCRepo.BCheckOutLogOnIndicator = true;

                //SCRepo.GetIndicatorSignal(this.HttpContext, 
                //    SCRepo.BCheckOutLogOnIndicator);

                //var AC = new AccountController();

                //AC.LogOn();

                return RedirectToAction("LogOn", "Account");
            }

            return RedirectToAction("AddressAndPayment", "Checkout");   
        }

        public IActionResult AddressAndPayment()
        {
            ViewData["isPost"] = false;

            return View();
        }

        [HttpPost]
        public IActionResult AddressAndPayment(Order OrderInfo)
        {
            bool NoEmptyProp = true;

            ShoppingCartRepo SCRepo = new ShoppingCartRepo();

            //AccountController Account = new AccountController();

            //Account.MigrateShoppingCart(OrderInfo, OrderInfo.Username);

            SCRepo.MigrateCart(this.HttpContext, OrderInfo);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            PropertyInfo[] properties = typeof(Order).GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(OrderInfo);

                if (value == null || string.IsNullOrWhiteSpace(value.ToString())) 
                {
                    NoEmptyProp = false;
                    break;
                }
            }

            if (NoEmptyProp == true) 
            {
                return RedirectToAction("Complete", "Checkout");
            }    
            else
            {
                ModelState.AddModelError("",
                    "One or more fields must be filled in.");

                ViewData["isPost"] = true;

                return View(OrderInfo);
            }
        }

        public IActionResult Complete(int id)
        {
            return View();
        }
    }
}