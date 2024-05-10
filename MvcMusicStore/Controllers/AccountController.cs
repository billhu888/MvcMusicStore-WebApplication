using Microsoft.AspNetCore.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using MvcMusicStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Repositories;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MvcMusicStore.Controllers
{
    public class AccountController : Controller
    {
        //public void MigrateShoppingCart(Order OrderInfo, String UserName)
        //{
        //    Order order = new Order();

        //    // Associate shopping cart items with logged-in user
        //    var cart = new ShoppingCartRepo(this.HttpContext);

        //    cart.MigrateCart(UserName, order);
        //    ViewData[ShoppingCart.CartSessionKey] = UserName;
        //}

        public IActionResult LogOnHeaderRow()
        {
            ShoppingCartRepo SCRepo = new ShoppingCartRepo();

            SCRepo.ConfirmIndicatorFromCheckout(this.HttpContext);
            string indicator = SCRepo.CheckIfLoggedOn(this.HttpContext);

            SCRepo.ConfirmLogOnFromHeader(this.HttpContext);

            if (indicator != "True")
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        public IActionResult LogOn()
        {
            ViewData["isPost"] = false;

            return View();
        }

        [HttpPost]
        public IActionResult LogOn(LogOn LogOn)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}        

            String CartId = "";

            AccountRepo accountrepo = new AccountRepo();
            ShoppingCartRepo SCRepo = new ShoppingCartRepo();

            SCRepo.ConfirmLoggedOn(this.HttpContext);

            string LogOnFromCheckout 
                = SCRepo.CheckFromCheckoutIndicator(this.HttpContext);

            string LogOnFromHeader 
                = SCRepo.CheckLogOnFromHeader(this.HttpContext);

            bool success = accountrepo.LogOn(LogOn);

            if (success)
            {
                ShoppingCartRepo SCRepo2 = new 
                    ShoppingCartRepo(this.HttpContext, 
                    LogOn.UserName);

                CartId = SCRepo2.GetCartId(this.HttpContext);

                SCRepo2.UpdateCartIdToUsername(CartId, LogOn.UserName);

                if (LogOnFromCheckout == "True")
                {
                    SCRepo2.CheckFromCheckoutIndicatorReturnFalse
                        (this.HttpContext);

                    if (LogOnFromHeader == "True")
                    {
                        return RedirectToAction(
                            "Index", "ShoppingCart");
                    }

                    return RedirectToAction(
                        "AddressAndPayment", "Checkout");
                }

                return RedirectToAction("Index", "Store");
            }
            else
            {
                ModelState.AddModelError("", 
                    "Username and/or password is incorrect.");

                ViewData["isPost"] = true;

                return View(LogOn); 
            }
        }

        public IActionResult Register()
        {
            ViewData["isPost"] = false;

            return View();
        }

        [HttpPost]
        public IActionResult Register(Register Register) 
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            String CartId = "";

            AccountRepo accountrepo = new AccountRepo();
            ShoppingCartRepo SCRepo = new ShoppingCartRepo();

            SCRepo.ConfirmLoggedOn(this.HttpContext);

            string LogOnFromCheckout
                = SCRepo.CheckFromCheckoutIndicator(this.HttpContext);

            string LogOnFromHeader
                = SCRepo.CheckLogOnFromHeader(this.HttpContext);

            bool success1 = accountrepo.RegisterEmail(Register);
            bool success2 = accountrepo.RegisterUserName(Register);
            bool success3 = accountrepo.RegisterPasswords(Register);

            if (success1 && success2 && success3)
            {
                bool success4 = accountrepo.AddAccount(Register);

                if (success4)
                {
                    ShoppingCartRepo SCRepo2 = new
                        ShoppingCartRepo(this.HttpContext, 
                        Register.UserName);

                    CartId = SCRepo2.GetCartId(this.HttpContext);

                    //SCRepo2.UpdateCartIdToUsername(CartId, Register.UserName);

                    if (LogOnFromCheckout == "True")
                    {
                        SCRepo2.CheckFromCheckoutIndicatorReturnFalse
                            (this.HttpContext);

                        if (LogOnFromHeader == "True")
                        {
                            return RedirectToAction(
                                "Index", "ShoppingCart");
                        }

                        return RedirectToAction(
                            "AddressAndPayment", "Checkout");
                    }

                    return RedirectToAction("Index", "Store");
                }
                else
                {
                    return View();
                }
            }
            else if (!success1)
            {
                ModelState.AddModelError("", "You already have an account.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test1";

                return View(Register);
            }
            else if (!success2)
            {
                ModelState.AddModelError("", "Username has already been taken. " +
                    "Please create another username. ");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test2";

                return View(Register);
            }
            else
            {
                ModelState.AddModelError("", "The passwords don't match. " +
                    "Please make sure the passwords match.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test3";

                return View(Register);
            }
        }

        public IActionResult ChangePassword()
        {
            ViewData["isPost"] = false;

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword ChangePassword)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            AccountRepo accountrepo = new AccountRepo();

            bool success1 = accountrepo.ChangePasswordConfirmUserPassword(ChangePassword);
            bool success2 = accountrepo.ChangePasswordsNewPassword(ChangePassword);

            if (success1 && success2)
            {
                bool success3 = accountrepo.ChangePasswordUpdatePassword(ChangePassword);

                if (success3)
                {
                    return RedirectToAction("Index", "Store");
                }
                else
                {
                    return View();
                }
            }
            else if (!success1) 
            {
                ModelState.AddModelError("", "The username and password don't match. ");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test1";

                return View(ChangePassword);
            }
            else
            {
                ModelState.AddModelError("", "The passwords don't match. ");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test2";

                return View(ChangePassword);
            }
        }
    }
}