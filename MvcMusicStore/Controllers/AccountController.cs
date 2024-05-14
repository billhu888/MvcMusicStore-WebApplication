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
        /// <summary>
        ///     GET: /Account/LogOnHeaderRow
        ///     When you click on the "Log On" in the header row, it checks if you are 
        ///         already logged in or not.
        /// </summary>
        /// 
        /// <returns>
        ///     If you are not aleady logged in, it takes you to the Account controller
        ///         and the LogOn method where you can log on
        ///     If you are already logged in, it takes you to the ShoppingCart controller
        ///         and the Index method where it displays everything in your cart
        /// </returns>

        public IActionResult LogOnHeaderRow()
        {
            ShoppingCartRepo ShoppingCartRepo = new ShoppingCartRepo();

            // Checks if you are already logged onto your account
            string indicator = ShoppingCartRepo.CheckIfLoggedOn(this.HttpContext);

            // Indicates that you are logging on after clicking on "Log On" in the header
            ShoppingCartRepo.ConfirmLogOnFromHeader(this.HttpContext);

            if (indicator != "True")
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        /// <summary>
        ///     GET: /Account/LogOn
        ///     This is the LogOn page where you log into your account.
        ///     If you do not have an account you can register to create an account.
        ///     If you forgot your password you can create a new password so you can log back in.
        /// </summary>
        /// 
        /// <returns>
        ///     It returns a view of the LogOn page where you can 
        ///     log into your account,
        ///     create an account if you don't have an account, 
        ///     or create a new password if you forgot your password.
        /// </returns>

        public IActionResult LogOn()
        {
            ViewData["isPost"] = false;

            return View();
        }

        /// <summary>
        ///     POST: /Account/LogOn
        ///     You are logging in and depending on if you are logging in while 
        ///         checking out or after clicking on the "Log On" in the header
        ///         and checking if the entered username and password match     
        /// </summary>
        /// 
        /// <param name="LogOn"> 
        ///     LogOn parameter in LogOn format with the username and password 
        ///         to see if the record can be found in the database
        /// </param>
        /// 
        /// <returns>
        ///     If you are logging in after clicking on the "Log On" in the header,
        ///         you go to the shopping cart and see all the items in the cart
        ///     If you are logging in while checking out, you continute with the
        ///         checkout process by submitting your contact and address information
        ///     If the username and password don't match, you are shown a messgae saying 
        ///         that the username and/or password is incorrect and have to retry
        /// </returns>

        [HttpPost]
        public IActionResult LogOn(LogOn LogOn)
        {
            String CartId = "";

            AccountRepo AccountRepo = new AccountRepo();
            ShoppingCartRepo ShoppingCartRepo = new ShoppingCartRepo();

            // Checks if you are logging in from the checkout as you need to have an account to checkout
            string LogOnFromCheckout = ShoppingCartRepo.CheckFromCheckoutIndicator(this.HttpContext);

            // Checks if you are logging in after clicking on the "Log On" from the header
            string LogOnFromHeader = ShoppingCartRepo.CheckLogOnFromHeader(this.HttpContext);

            // LogOn into your account
            bool success = AccountRepo.LogOn(LogOn);

            if (success)
            {
                // Sets the session key indicating you are logged into your account
                ShoppingCartRepo.ConfirmLoggedOn(this.HttpContext);

                // If the user has an ongoing shopping cart session, it will find the cart and its items
                ShoppingCartRepo ShoppingCartRepo2 = new ShoppingCartRepo(this.HttpContext, LogOn.UserName);

                CartId = ShoppingCartRepo2.GetCartId(this.HttpContext);

                // Change the CartId to your username
                ShoppingCartRepo2.UpdateCartIdToUsername(CartId, LogOn.UserName);

                // Indicates that you are not logging in while checking out
                if (LogOnFromCheckout == "True")
                {
                    // Switch the logging in from checking out back to false
                    ShoppingCartRepo2.CheckFromCheckoutIndicatorReturnFalse(this.HttpContext);

                    return RedirectToAction("AddressAndPayment", "Checkout");
                }
                // Indicates you are logging in after clicking on the "Log On" in the header
                else if (LogOnFromHeader == "True")
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }

                return RedirectToAction("Index", "Store");
            }
            else
            {
                // If the username and/or password is incorrect, you are notified of that error
                ModelState.AddModelError("", "Username and/or password is incorrect.");

                ViewData["isPost"] = true;

                return View(LogOn); 
            }
        }

        /// <summary>
        ///     GET: /Account/Register
        ///     You register (create) an account if you don't have one
        /// </summary>
        /// 
        /// <returns>
        ///     The webpage that allows you to create an account.
        /// </returns>
        
        public IActionResult Register()
        {
            ViewData["isPost"] = false;

            return View();
        }

        /// <summary>
        ///     POST: /Account/Register
        ///     When you create a new account, you have to enter your email, username, password, and confirm password
        ///         and checks if the information you entered is valid and can be used for your new account
        /// </summary>
        /// 
        /// <param name="Register">
        ///     The Register parameter of Register type which contain the email, username, password, and confirm password
        ///         that you submitted when creating your new account
        /// </param>
        /// 
        /// <returns>
        ///     If the email is not in the database, the username has not been previously taken, password and confirm password match,
        ///         and the account has succesfully been created and added into the database, you are taken to the shopping cart
        /// </returns>
        /// 

        [HttpPost]
        public IActionResult Register(Register Register) 
        {
            String CartId = "";

            AccountRepo AccountRepo = new AccountRepo();
            ShoppingCartRepo ShoppingCartRepo = new ShoppingCartRepo();       

            // Checks if you are logging in from the checkout as you need to have an account to checkout
            string LogOnFromCheckout = ShoppingCartRepo.CheckFromCheckoutIndicator(this.HttpContext);

            // Checks if you are logging in after clicking on the "Log On" from the header
            string LogOnFromHeader = ShoppingCartRepo.CheckLogOnFromHeader(this.HttpContext);

            // Checks if the email address has already been taken or not
            bool success1 = AccountRepo.RegisterEmail(Register);

            // Checks if the email address is valid or not
            bool success2 = AccountRepo.CheckEmailValid(Register.Email);

            // Checks if the username has already been taken or not
            bool success3 = AccountRepo.RegisterUserName(Register);

            // Checks is the username is at least 6 characters long
            bool success4 = AccountRepo.CheckUsernameLength(Register.UserName);

            // Checks if the password and confirm password match
            bool success5 = AccountRepo.RegisterPasswords(Register);

            // Checks if the password is at least 6 characters long
            bool success6 = AccountRepo.CheckPasswordLength(Register.Password);

            if (success1 && success2 && success3 && success4 && success5 && success6)
            {
                // To confirm that the new acount has been created and added to the database
                bool success7 = AccountRepo.AddAccount(Register);

                if (success7)
                {
                    // Sets the session key indicating you are logged into your account
                    ShoppingCartRepo.ConfirmLoggedOn(this.HttpContext);

                    ShoppingCartRepo ShoppingCartRepo2 = new ShoppingCartRepo(this.HttpContext, Register.UserName);

                    // Get your Cart ID
                    CartId = ShoppingCartRepo2.GetCartId(this.HttpContext);

                    // Change the CartId to your username
                    ShoppingCartRepo2.UpdateCartIdToUsername(CartId, Register.UserName);

                    if (LogOnFromCheckout == "True")
                    {
                        // Switch the logging in from checking out back to false
                        ShoppingCartRepo2.CheckFromCheckoutIndicatorReturnFalse(this.HttpContext);

                        return RedirectToAction("AddressAndPayment", "Checkout");
                    }
                    else if (LogOnFromHeader == "True")
                    {
                        return RedirectToAction("Index", "ShoppingCart");
                    }

                    return RedirectToAction("Index", "Store");
                }
                else
                {
                    return View();
                }
            }
            // Error message if you already have an account
            else if (!success1)
            {
                ModelState.AddModelError("", "You already have an account.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test1";

                return View(Register);
            }
            // Error message if email is not valid
            else if (!success2)
            {
                ModelState.AddModelError("", "Email is not valid. Please enter a valid email address ");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test2";

                return View(Register);
            }
            // Error message if username has already been taken
            else if (!success3)
            {
                ModelState.AddModelError("", "Username has already been taken. Please enter another username.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test3";

                return View(Register);
            }
            // Error message is username is not at least 6 characters long
            else if (!success4)
            {
                ModelState.AddModelError("", "Please enter a username that is at least 6 characters long.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test4";

                return View(Register);
            }
            // Error message if the password and confirm password don't match
            else if (!success5)
            {
                ModelState.AddModelError("", "The passwords don't match. Please make sure the passwords match.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test5";

                return View(Register);
            }
            // Error message if the password is not at least 6 characters long
            else
            {
                ModelState.AddModelError("", "Please enter a password that is at least 6 characters long.");
                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test6";

                return View(Register);
            }
        }

        /// <summary>
        ///     GET: /Account/ChangePassword
        ///     This allows you to change your password
        /// </summary>
        /// 
        /// <returns>
        ///     Takes you to the page where you can change your password
        /// </returns>
        /// 
        public IActionResult ChangePassword()
        {
            ViewData["isPost"] = false;

            return View();
        }

        /// <summary>
        ///     POST: Account/ChangePassword
        ///     Allows you to set a new password for your account
        /// </summary>
        /// 
        /// <param name="ChangePassword">
        ///     The ChangePassword Paramater of the ChangePasswordType contains the Username, 
        ///         Current Password, New Password, and Confirm New Password
        /// </param>
        /// 
        /// <returns>
        ///     If password is successfully updated it takes you to the store index page
        ///     Otherwise it will tell you what errors you have made and need to fix
        /// </returns>
        
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword ChangePassword)
        {
            AccountRepo AccountRepo = new AccountRepo();

            // Confirm that the username and password match an existing account
            bool success1 = AccountRepo.ChangePasswordConfirmUserPassword(ChangePassword);

            // Check that the New Password and Confirm New Password match
            bool success2 = AccountRepo.ChangePasswordsNewPassword(ChangePassword);

            if (success1 && success2)
            {
                // Changes the password of your account
                bool success3 = AccountRepo.ChangePasswordUpdatePassword(ChangePassword);

                if (success3)
                {
                    return RedirectToAction("Index", "Store");
                }
                else
                {
                    return View();
                }
            }
            // Error message if the username and password don't match an existing account
            else if (!success1) 
            {
                ModelState.AddModelError("", "The username and password don't match. ");

                ViewData["isPost"] = true;
                ViewData["Indicator"] = "Test1";

                return View(ChangePassword);
            }
            // Error message if the New Password and Confirm New Password don't match
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