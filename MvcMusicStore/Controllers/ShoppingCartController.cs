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
        /// <summary>
        ///     GET: /ShoppingCart/Index
        ///     Shows all the items in your shopping cart and your total price
        /// </summary>
        /// 
        /// <returns>
        ///     All the items in your shopping cart and your total price
        /// </returns>
        
        public IActionResult Index()
        {
            var cart = new ShoppingCartRepo(this.HttpContext);

            var ViewModel = new ShoppingCartItems();

            ViewModel.CartItems = new List<Cart>();

            // Gets all the items in your shopping cart
            bool success1 = cart.GetCartItems(this.HttpContext, ViewModel.CartItems);

            // Get how many items are in your shopping cart
            bool success2 = cart.GetCartItemCount(ViewModel);

            // Gets the total price of all of the items in your shopping cart
            bool success3 = cart.GetTotal(ViewModel);

            if (success1 && success3)
            {
                return View(ViewModel);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        ///     GET: /ShoppingCart/AddToCart
        ///     Adds an album to your shopping cart
        /// </summary>
        /// 
        /// <param name="AlbumId">
        ///     AlbumId parameter contains the AlbumId of the album
        ///         you want to add to your cart
        /// </param>
        /// 
        /// <returns>
        ///     If item successfully added to your cart, 
        ///         will show the updated shopping cart
        /// </returns>
        
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

        /// <summary>
        ///     POST: /ShoppingCart/RemoveFromCartAJAX
        ///     Removes an album from your shopping cart using AJAX
        ///     This way only the necessary parts are updated and 
        ///         not having to refresh the entire page
        /// </summary>
        /// 
        /// <param name="RecordId">
        ///     RecordId parameter contains the RecordId of the album
        ///         you want to remove from your cart
        /// </param>
        /// 
        /// <returns>
        ///     If item successfully removed from your cart, 
        ///         a JSON string consisting of the updated data 
        ///         will show the updated shopping cart data
        ///         without refreshing the entire page
        /// </returns>

        [HttpPost]
        public IActionResult RemoveFromCartAJAX(int RecordId)
        {
            var CartRemove = new ShoppingCartRemoveItem();
            var ViewModel = new ShoppingCartItems();   
            
            // Make sure there is a cart and if not create one and its ID
            var cart = new ShoppingCartRepo(this.HttpContext);

            // Get the ID of the record in the cart that the item was deleted from
            CartRemove.DeleteId = RecordId;

            // Retrieve the items in the cart
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
                return Json( new { data = removedmodel });
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        ///     POST: /ShoppingCart/CartTotalItemsCountAJAX
        ///     Get the total number of items in the cart using AJAX
        ///     Will update the total number of items in the cart
        ///         without having to refresh the entire page
        /// </summary>
        /// 
        /// <returns>
        ///     If succesfully counting the number of items in the cart, 
        ///         a JSON string consisting of the number of items
        ///         in the cart will show without refreshing the entire page
        /// </returns>

        [HttpPost]
        public IActionResult CartTotalItemsCountAJAX()
        {
            var CartRemove = new ShoppingCartRemoveItem();
            var ViewModel = new ShoppingCartItems();

            // Make sure there is a cart and if not create one and its ID
            var cart = new ShoppingCartRepo(this.HttpContext);

            // Retrieve the items in the cart
            ViewModel.CartItems = new List<Cart>();

            bool success1 = cart.GetCartItemsHeaderAJAX(this.HttpContext, ViewModel.CartItems);
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
    }
}