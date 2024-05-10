using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models
{
    public partial class Order
    {
        [Required]
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public System.DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Order Details")]
        public List<OrderDetail> OrderDetails { get; set; }
    }
}