using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using MvcMusicStore.Controllers;
using Microsoft.Data.SqlClient;
using System.Data;
using MvcMusicStore.Repositories;

namespace MvcMusicStore.Repositories
{
    class ShoppingCart : BaseRepo
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        // Must add builder.Services.AddSession() and  app.UseSession() in Program.cs to create session
        // Session isn't auto created, need config.
        public string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        // Default Constructor
        public ShoppingCart()
        {}

        //
        public ShoppingCart(HttpContext context)
        {
            ShoppingCartId = GetCartId(context);
        }

        //
        public static ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // We're using HttpContextBase to allow access to cookies.
        //You can use extension methods (Microsoft.AspNetCore.Http.Extensions) to get or set string session keys:
        public string GetCartId(HttpContext context)
        {
            var session = context.Session;

            if (!session.Keys.Contains(CartSessionKey))
            {
                var userName = context.User.Identity.Name;

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    session.SetString(CartSessionKey, userName);
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();

                    session.SetString(CartSessionKey, tempCartId.ToString());          
                    session.SetString("Bill", "Bill HU");
                    session.SetString("YIjun", "Yijun HShiU");
                }
            }
            return session.GetString(CartSessionKey);
        }

        #region Access Database   
        #endregion
    }
}