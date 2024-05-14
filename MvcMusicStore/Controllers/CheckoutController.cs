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
        /// <summary>
        ///     GET: /Checkout 
        /// </summary>
        /// 
        /// <returns>
        ///     The checkout page index
        /// </returns>
        
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     GET: /Checkout/AddressPayment
        ///     Checks to make sure you are logged in or not before 
        ///         you submit your address and payment information
        /// </summary>
        /// 
        /// <returns>
        ///     If you are already logged in, you are taken to the Checkout controller 
        ///         and AddressAndPayment method to submit your address and payment information 
        ///     If you are not logged in yet, you have to first log in before you can 
        ///         submit your address and payment information
        /// </returns

        public IActionResult AddressPayment()
        {
            var order = new Order();
            ShoppingCartRepo ShoppingCartRepo = new ShoppingCartRepo();

            // Make it clear that this is from checkout
            ShoppingCartRepo.ConfirmIndicatorFromCheckout(this.HttpContext);

            // CHeck to see if you are already logged on
            string indicator = ShoppingCartRepo.CheckIfLoggedOn(this.HttpContext);

            if (indicator != "True")
            {
                return RedirectToAction("LogOn", "Account");
            }

            return RedirectToAction("AddressAndPayment", "Checkout");   
        }

        /// <summary>
        ///     GET: /Checkout/AddressAndPayment
        ///     Where you input and submit your address and payment information
        /// </summary>
        /// 
        /// <returns>
        ///     The form where you sybmit your address and payment information
        /// </returns>

        public IActionResult AddressAndPayment()
        {
            ViewData["isPost"] = false;

            return View();
        }

        /// <summary>
        ///     POST: /Checkout/AddressAndPayment
        ///     Make sure that all the address and payment information fields are filled in
        ///     After that delete the shopping cart record from the shopping cart database
        ///         and move the data to the Orders and OrderDetails databases
        /// </summary>
        /// 
        /// <param name="OrderInfo">
        ///     The OrderInfo parameter of the Order type propreties have all the
        ///         address and payment information
        /// </param>
        /// 
        /// <returns>
        ///     If all the address and payment information fields are filled, go to the
        ///         Complete method to finish your order
        ///     If at least one of the address and payment information fields are missing,
        ///         it will return an error message to you to fill the field(s) in
        /// </returns>

        [HttpPost]
        public IActionResult AddressAndPayment(Order OrderInfo)
        {
            bool NoEmptyProp = true;

            ShoppingCartRepo ShoppingCartRepo = new ShoppingCartRepo();

            // Deletes the shopping cart record from the shopping cart database
            //    and moves the data to the Orders and OrderDetails databases 
            ShoppingCartRepo.MigrateCart(this.HttpContext, OrderInfo);

            // Compiles a list of all the properties in the Order model
            PropertyInfo[] properties = typeof(Order).GetProperties();

            // Goes through each proprety in the Order model
            foreach (var property in properties)
            {
                // Retrieves the value of each proprety
                var value = property.GetValue(OrderInfo);

                // Checks to make sure every proprety has a value
                if (value == null || string.IsNullOrWhiteSpace(value.ToString())) 
                {
                    // If any proprety does not have a value, it is made clear
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
                ModelState.AddModelError("", "One or more fields are empty and must be filled in.");

                ViewData["isPost"] = true;

                return View(OrderInfo);
            }
        }

        /// <summary>
        ///     GET: /Checkout/Complete
        ///     Confirmation that you have finished checking out your order
        /// </summary>
        ///  
        /// <returns>
        ///     The page confirming your order has been successfully submitted
        /// </returns>
        
        public IActionResult Complete()
        {
            return View();
        }
    }
}