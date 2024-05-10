using Microsoft.AspNetCore.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using MvcMusicStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        //public IActionResult Index()
        //{
        //    var cart1 = new ShoppingCart();
        //    ViewBag.CardId = "Test1";

        //    cart1.ShoppingCartId = cart1.GetCartId(this.HttpContext);

        //    var cart2 = ShoppingCart.GetCart(this.HttpContext);

        //    ShoppingCart cart3 = new ShoppingCart(this.HttpContext);

        ////    // Set up our ViewModel
        ////    /*var viewModel = new ShoppingCartViewModel
        ////    {
        ////        CartItems = cart.GetCartItems(),
        ////        CartTotal = cart.GetTotal()
        ////    };
        ////    // Return the view
        ////    return View(viewModel);*/

        //    //view ShoppingCartId assigned in session
        //    ViewBag.CardID2 = cart2.ShoppingCartId;
        //    ViewData["CardID1"] = cart1.ShoppingCartId;
        //    ViewData["CardID3"] = cart3.ShoppingCartId;
        //    return View();
        //}

        public IActionResult Index()
        {
            var cart = new ShoppingCartRepo(this.HttpContext);

            var ViewModel = new ShoppingCartItems();
            ViewModel.CartItems = new List<Cart>();

            bool success1 = cart.GetCartItems(this.HttpContext, ViewModel.CartItems);
            bool success2 = cart.GetTotal(ViewModel);

            if (success1 && success2)
            {
                return View(ViewModel);
            }
            else
            {
                return View();
            }
        }

        public IActionResult AddToCart(int AlbumId)
        {
            var cart = new ShoppingCartRepo(this.HttpContext);        

            bool success = cart.AddToCart(this.HttpContext, AlbumId);

            if (success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult RemoveFromCart(int AlbumId)
        {
            var cart = new ShoppingCartRepo(this.HttpContext);

            bool success = cart.RemoveFromCart(AlbumId);

            if (success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCartAJAX(int RecordId)
        {
            var CartRemove = new ShoppingCartRemoveItem();
            var ViewModel = new ShoppingCartItems();      
            var cart = new ShoppingCartRepo(this.HttpContext);

            CartRemove.DeleteId = RecordId;

            ViewModel.CartItems = new List<Cart>();

            bool success1 = cart.GetAlbumTitleAJAX(CartRemove, RecordId);
            bool success2 = cart.RemoveFromCartAJAX(CartRemove, RecordId);
            bool success3 = cart.GetCartItems(this.HttpContext, ViewModel.CartItems);
            bool success4 = cart.GetTotalAJAX(CartRemove, ViewModel);
            bool success5 = cart.GetCountAJAX(CartRemove, ViewModel);

            var removedmodel = new ShoppingCartRemoveItem
            {
                Message = CartRemove.Message,
                CartTotal = CartRemove.CartTotal,
                CartCount = CartRemove.CartCount,
                ItemCount = CartRemove.ItemCount,
                DeleteId = CartRemove.DeleteId
            };

            if (success1 && success2 && success3 && success4 && success5)
            {
                // This results in 2 layers 
                // 1st layer is the data
                // 2nd layer is each key (proprety) (data.cartTotal)

                //{
                    //"data":
                    //{
                        //"message":"For Those About To Rock We Salute You
                            //has been removed from your shopping cart.",
                        //"cartTotal":0,
                        //"cartCount":0,
                        //"itemCount":0,
                        //"deleteId":88
                    //}
                //}

                // The Json() method is used to serialize an object into JSON format
                    // and return it as a JsonResult from the controller action.
                    // It's commonly used to send data back to the client in AJAX calls.
                //new { data = removedmodel }: This part of the code creates
                    //an anonymous object in C#
                //new { ... }: This syntax creates a new anonymous object.
                    //data = removedmodel: Here, data is a property of
                    //the anonymous object,
                    //and removedmodel is the value assigned to this property.
                //The property name data is used to hold the data that
                    //will be serialized into JSON.
                //Return Statement: The return Json(...); statement then returns
                    //this anonymous object serialized as JSON back to the client.
                    //In this case, removedmodel(or whatever removedmodel represents)
                    //will be serialized into JSON format and sent back to the client.

                return Json( new { data = removedmodel });

                // This only results in 1 layer so every key is 
                // processed one-by-one

                //{
                    //"message":"Let There Be Rock has been removed from
                        //your shopping cart.",
                    //"cartTotal":0,
                    //"cartCount":0,
                    //"itemCount":0,
                    //"deleteId":88
                //}

                //return Json( removedmodel );
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult CartTotalItemsCountAJAX()
        {
            var CartRemove = new ShoppingCartRemoveItem();
            var ViewModel = new ShoppingCartItems();
            var cart = new ShoppingCartRepo(this.HttpContext);

            ViewModel.CartItems = new List<Cart>();

            bool success1 = cart.GetCartItemsHeaderAJAX
                (this.HttpContext, ViewModel.CartItems);
            bool success2 = cart.GetCountHeaderAJAX(CartRemove, ViewModel);

            var removedmodel = new ShoppingCartRemoveItem
            {
                CartCount = CartRemove.CartCount
            };

            if (success1 && success2)
            {
                return Json(new { data = removedmodel });
            }
            else
            {
                return View();
            }
        }

        //public ActionResult CartSummary()
        //{
        //    return PartialView("_CartSummary");
        //}
    }
}