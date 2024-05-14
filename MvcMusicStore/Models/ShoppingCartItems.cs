using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models
{
    public class ShoppingCartItems
    {
        public List<Cart> CartItems { get; set; }
        public int CartTotalItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}