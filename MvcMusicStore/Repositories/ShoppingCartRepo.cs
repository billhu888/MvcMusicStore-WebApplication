using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcMusicStore.Models;
using System.Data;
using MvcMusicStore.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MvcMusicStore.Repositories;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using static System.Collections.Specialized.BitVector32;

namespace MvcMusicStore.Repositories
{
    class ShoppingCartRepo : BaseRepo
    {
        public const String CheckOutLogOnKey = "CheckOutLogOn";
        public bool BCheckOutLogOnIndicator { get; set; }
        public String SCheckOutLogOnIndicator { get; set; }

        public const String LogOnFromHeader = "LogOnFromHeader";
        public String LogOnFromHeaderIndicator { get; set; }

        public const String ComeFromCheckout = "ComeFromCheckout";
        public String LogOnCheckoutIndicator { get; set; }

        public const String AlreadyLogIn = "AlreadyLogIn";
        public String AlreadyLogInIndicator { get; set; }

        public const String UserNameKey = "UserName";
        public String UserNameOrder { get; set; }

        public const String CartSessionKey = "CartId";
        public String ShoppingCartId { get; set; }

        /// <summary>
        ///    Default Constructor
        /// </summary>
        
        public ShoppingCartRepo() 
        { }

        /// <summary>
        ///     Constructor to get the Cart ID
        /// </summary>
        /// 
        /// <param name="context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>

        public ShoppingCartRepo(HttpContext context)
        {
            // Get the ID of the shopping cart
            ShoppingCartId = GetCartId(context);
        }

        /// <summary>
        ///     Constructor if you are logging in and you have a
        ///         cart and its ID that have not been checked out yet
        /// </summary>
        /// 
        /// <param name="context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="UserName">
        ///     The user's username
        /// </param>

        public ShoppingCartRepo(HttpContext context, String UserName)
        {
            // Get the ID of the user's shopping cart
            UserNameOrder = GetCartId(context, UserName);
        }

        /// <summary>
        ///     Get the shopping cart ID for the cart from the CartSessionKey
        ///     If there is no ID for the cart a cart ID is generated
        ///         and assigned to the CartSessionKey
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <returns>
        ///     The Cart ID assigned to the CartSessionKey
        /// </returns>

        public String GetCartId(HttpContext Context)
        {
            var session = Context.Session;

            // Checks if the session already has a Cart ID in CartSessionKey
            // If the session doesn't have a Cart ID it creates one and assigns
            //    it to CartSessionKey and then gets it from CartSessionKey
            // If the session already has a Cart ID it gets it from CartSessionKey
            if (!session.Keys.Contains(CartSessionKey))
            {
                var userName = Context.User.Identity.Name;

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    session.SetString(CartSessionKey, userName);
                }
                else
                {
                    // GUILD stands for "Globally Unique Identifier"
                    // Creates a new Cart ID
                    Guid tempCartId = Guid.NewGuid();

                    // Sets the session Cart ID to the
                    // just generated Cart ID in string format
                    session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }

            // Returns the value of the Cart ID
            return session.GetString(CartSessionKey);
        }

        /// <summary>
        ///     Retrieves the username from UserNameKey
        ///     If UserNameKey doesnt have a username it assigns it the UserName
        ///        and then gets it from UserNameKey
        ///     If UserNameKey already has a username it gets it from UserNameKey
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="UserName">
        ///     The user's username
        /// </param>
        /// 
        /// <returns>
        ///     The user's username from UserNameKey
        /// </returns>

        public String GetCartId(HttpContext Context, String UserName)
        {
            var session = Context.Session;

            // Checks if there is not already UserNameKey
            if (!session.Keys.Contains(UserNameKey))
            {
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    session.SetString(UserNameKey, UserName);
                }
                else
                {
                    // Sets UserNameKey to the UserName
                    session.SetString(UserNameKey, UserName);
                }
            }

            // Returns the value of UserNameKey
            return session.GetString(UserNameKey);
        }

        /// <summary>
        ///     Confirms this is coming from checkout
        ///     Sets the ComeFromCheckout key to true saying it is from checkout
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>

        public void ConfirmIndicatorFromCheckout(HttpContext Context)
        {
            var session = Context.Session;

            // Sets the ComeFromCheckout key to true saying it is from checkout
            session.SetString(ComeFromCheckout, "True");
        }

        /// <summary>
        ///     Checks if you are logging in from the checkout as you need to have 
        ///        an account to checkout
        ///     Sets the ComeFromCheckout key to true indicating it is coming from checkout
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <returns>
        /// 
        /// </returns>

        public string CheckFromCheckoutIndicator(HttpContext Context)
        {
            var session = Context.Session;

            // Sets the ComeFromCheckout key to true indicating it is coming from checkout
            return session.GetString(ComeFromCheckout);
        }

        /// <summary>
        ///     Switch the logging in from checking out back to false
        ///     Sets the ComeFromCheckout back to false
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>

        public void CheckFromCheckoutIndicatorReturnFalse(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(ComeFromCheckout, "True");
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>

        public void ConfirmLoggedOn(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(AlreadyLogIn, "True");
        }

        /// <summary>
        ///     Checks if you have already logged on by checking the AlreadyLogIn key
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <returns>
        ///     The result of the AlreadyLogIn key indicating if you are logged on or not
        /// </returns>

        public string CheckIfLoggedOn(HttpContext Context)
        {
            var session = Context.Session;

            // Checks the AlreadyLogIn key indicating if you are already logged in or not
            return session.GetString(AlreadyLogIn);
        }

        /// <summary>
        ///     Confirmss that you are logging in from the header 
        ///     Sets the LogOnFromHeader key to true
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>

        public void ConfirmLogOnFromHeader(HttpContext Context)
        {
            var session = Context.Session;

            // Sets the LogOnFromHeader key to true indicating you are logging in from the header
            session.SetString(LogOnFromHeader, "True");
        }

        /// <summary>
        ///     Checks if you are logging in from the header by checking the LogOnFromHeader key value
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <returns>
        ///     Returns the value of the LogOnFromHeader key
        ///     True means you are logging in from the header
        /// </returns>

        public string CheckLogOnFromHeader(HttpContext Context)
        {
            var session = Context.Session;

            // Checks and returns the value of the LogOnFromHeader key to see
            //    if you are logging in from the header or not
            return session.GetString(LogOnFromHeader);
        }

        /// <summary>
        ///     When you log into you account, it changes the CartId 
        ///        from the CartId to your username
        /// </summary>
        /// 
        /// <param name="CartId">
        ///     The Id of the cart you want to change to your username
        /// </param>
        /// 
        /// <param name="UserName">
        ///     Your username which will replace the cartID
        /// </param>

        public void UpdateCartIdToUsername(String CartId, String UserName)
        {
            try
            {
                if (Connect())
                {
                    String sql1 =
                      $" UPDATE Cart " +
                      $" SET CartId = '{UserName}' " +
                      $" WHERE CartId = '{CartId}' ";

                    SqlCommand cmd = new SqlCommand(sql1, connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        /// <summary>
        ///     After you checkout, it moves the cart record to the Order and Order Details tables
        ///        and deletes the cart record
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="order">
        ///     Holds all your personal, contact, and payment information
        /// </param>

        public void MigrateCart(HttpContext Context, Order order)
        {
            try
            {
                if (Connect())
                {
                    var session = Context.Session;
                    decimal TotalCost = 0.00M;

                    // Sets the value of the UserNameKey key which holds your username to the 
                    //    UserNameKey proprerty
                    order.Username = session.GetString(UserNameKey);
                    
                    // Changes your cart ID to your username
                    UpdateCartIdToUsername(session.GetString(CartSessionKey), order.Username);                             

                    String sql1 =
                        $" SELECT Cart.AlbumId, Cart.Count, Album.Price " +
                        $" FROM Cart " +
                        $" INNER JOIN ALBUM on Cart.AlbumId = Album.AlbumId " +
                        $" WHERE cartId = '{order.Username}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql1, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        OrderDetail orderDetail;

                        order.OrderDetails = new List<OrderDetail>();

                        foreach (DataRow row in dt.Rows)
                        {
                            orderDetail = new OrderDetail() 
                            {
                                AlbumId = (int)row["AlbumId"],
                                Quantity = (int)row["Count"],
                                UnitPrice = (decimal)row["Price"]
                            };

                            order.OrderDetails.Add(orderDetail);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No items ordered");
                    }

                    // Calculates the total cost of your order
                    foreach (var item in order.OrderDetails)
                    {
                        TotalCost = TotalCost + item.Quantity * item.UnitPrice;
                    }

                    String sql2 =
                        $" SELECT Count(*) " +
                        $" FROM Orders ";

                    SqlCommand cmd = new SqlCommand(sql2, connection);
                    int rowCount = (int)cmd.ExecuteScalar();

                    // Gets the OrderId of your order
                    if (rowCount == 0)
                    {
                        order.OrderId = 1;
                    }
                    else
                    {
                        String sql3 =
                            $" SELECT MAX(OrderId) AS MAXOrderId " +
                            $" FROM Orders ";

                        DataTable dt2 = new DataTable();
                        SqlDataAdapter da2 = new SqlDataAdapter();
                        da2.SelectCommand = new SqlCommand(sql3, connection);
                        da2.Fill(dt2);

                        if (dt2.Rows.Count > 0)
                        {
                            DataRow row1 = dt2.Rows[0];

                            order.OrderId = (int)row1["MAXOrderId"] + 1;
                        }
                    }

                    // Sets the Total proprety to the cost of your order
                    order.Total = TotalCost;

                    DateTime dateTime = DateTime.Now;

                    // Adds the Order information into the Orders database
                    String sql4 =
                        $" INSERT INTO ORDERS (" +
                        $"    OrderId, Username, FirstName, LastName, Address, " +
                        $"    City, State, PostalCode, Country, Phone, " +
                        $"    Total, DateCreated, Email) " +
                        $" Values ('{order.OrderId}', '{session.GetString(UserNameKey)}', " +
                        $"    '{order.FirstName}', '{order.LastName}', " +
                        $"    '{order.Address}', '{order.City}', " +
                        $"    '{order.State}', '{order.PostalCode}', '{order.Country}', " +
                        $"    '{order.Phone}', {order.Total}, " +
                        $"    '{dateTime}', '{order.Email}') ";

                    SqlCommand command = new SqlCommand(sql4, connection);
                    command.ExecuteNonQuery();

                    // Adds every item that was ordered in your order into the 
                    //    Order Details database
                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = order.OrderId;

                        String sql5 =
                            $" SELECT Count(*) " +
                            $" FROM  OrderDetails ";

                        SqlCommand cmd2 = new SqlCommand(sql5, connection);
                        int rowCount2 = (int)cmd2.ExecuteScalar();

                        if (rowCount2 == 0)
                        {
                            item.OrderDetailId = 1;
                        }
                        else
                        {
                            String sql6 =
                                $" SELECT MAX(OrderDetailId) AS MAXOrderDetailId" +
                                $" FROM OrderDetails ";

                            DataTable dt3 = new DataTable();
                            SqlDataAdapter da3 = new SqlDataAdapter();
                            da3.SelectCommand = new SqlCommand(sql6, connection);
                            da3.Fill(dt3);

                            if (dt3.Rows.Count > 0)
                            {
                                DataRow row1 = dt3.Rows[0];

                                item.OrderDetailId = (int)row1["MAXOrderDetailId"] + 1;
                            }
                        }

                        String sql7 =
                            $" INSERT INTO OrderDetails ( " +
                            $" OrderDetailId, OrderId, AlbumId, Quantity, UnitPrice) " +
                            $" Values ({item.OrderDetailId}, {item.OrderId}, " +
                            $" {item.AlbumId}, {item.Quantity}, {item.UnitPrice}) ";

                        SqlCommand command2 = new SqlCommand(sql7, connection);
                        command2.ExecuteNonQuery();
                    }

                    // Deletes your shopping cart from the Cart database
                    String sql8 =
                        $" DELETE FROM Cart" +
                        $" WHERE CartId = '{order.Username}' ";

                    SqlCommand command3 = new SqlCommand(sql8, connection);
                    command3.ExecuteNonQuery();

                    session.SetString(ComeFromCheckout, "False");
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        /// <summary>
        ///     Adds each item you ordered to your shopping cart (cart database)
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="AlbumId">
        ///     The Id of the album you just added to your cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the album was successfully added to your cart (cart database)
        /// </returns>

        public bool AddToCart(HttpContext Context, int AlbumId)
        {
            bool success = false;
            int RecordId = 0;
            int ItemCount = 0;
            DateTime dateTime = DateTime.Now;
            var session = Context.Session;

            try
            {
                if (Connect())
                {
                    String sql1 =
                        $" SELECT COUNT(*) " +
                        $" FROM CART ";

                    SqlCommand cmd = new SqlCommand(sql1, connection);
                    int rowCount = (int)cmd.ExecuteScalar();

                    if (rowCount == 0)
                    {
                        RecordId = 0;
                    } 
                    else
                    {
                        String sql2 =
                            $" SELECT MAX (RecordID) AS MaxRecordID " +
                            $" FROM CART ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql2, connection);
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row1 = dt.Rows[0];

                            RecordId = (int)row1["MaxRecordID"] + 1;
                        }
                    }

                    // Checks if you are already logged in so it knows if it is to find
                    // your shopping cart ID or your username in the Cart database
                    if (session.GetString(AlreadyLogIn) == "True") 
                    {
                        String sql3 =
                            $" SELECT Count(*) AS Records " +
                            $" FROM Cart " +
                            $" WHERE AlbumId = {AlbumId} AND " +
                            $"    CartId = '{session.GetString(UserNameKey)}' ";

                        SqlCommand cmd2 = new SqlCommand(sql3, connection);
                        int rowCount2 = (int)cmd2.ExecuteScalar();

                        // Checks if it is the 1st item of that album being added to your cart 
                        if (rowCount2 == 0)
                        {
                            String sql4 =
                                $" INSERT INTO CART " +
                                $"    (RecordId, CartId, AlbumId, Count, DateCreated) " +
                                $" VALUES ({RecordId}, '{session.GetString(UserNameKey)}', " +
                                $"    {AlbumId}, 1, '{dateTime}') ";

                            SqlCommand command = new SqlCommand(sql4, connection);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            String sql5 =
                                $" SELECT Count " +
                                $" FROM Cart " +
                                $" WHERE AlbumId = {AlbumId} " +
                                $"    AND CartId = '{session.GetString(UserNameKey)}' ";

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = new SqlCommand(sql5, connection);
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                DataRow row1 = dt.Rows[0];

                                ItemCount = (int)row1["Count"] + 1;
                            }

                            String sql6 =
                                $" UPDATE Cart " +
                                $" SET Count = {ItemCount} " +
                                $" WHERE AlbumId = {AlbumId} " +
                                $"    AND CartId = '{session.GetString(UserNameKey)}' ";

                            SqlCommand command = new SqlCommand(sql6, connection);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        String sql3 =
                            $" SELECT Count(*) AS Records " +
                            $" FROM Cart " +
                            $" WHERE AlbumId = {AlbumId} AND CartId = '{ShoppingCartId}' ";

                        SqlCommand cmd2 = new SqlCommand(sql3, connection);
                        int rowCount2 = (int)cmd2.ExecuteScalar();

                        // Checks if it is the 1st item of that album being added to your cart 
                        if (rowCount2 == 0)
                        {
                            String sql4 =
                                $" INSERT INTO CART " +
                                $"    (RecordId, CartId, AlbumId, Count, DateCreated) " +
                                $" VALUES ({RecordId}, '{ShoppingCartId}', " +
                                $"    {AlbumId}, 1, '{dateTime}') ";

                            SqlCommand command = new SqlCommand(sql4, connection);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            String sql5 =
                                $" SELECT Count " +
                                $" FROM Cart " +
                                $" WHERE AlbumId = {AlbumId} AND CartId = '{ShoppingCartId}' ";

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = new SqlCommand(sql5, connection);
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                DataRow row1 = dt.Rows[0];

                                ItemCount = (int)row1["Count"] + 1;
                            }

                            String sql6 =
                                $" UPDATE Cart " +
                                $" SET Count = {ItemCount} " +
                                $" WHERE AlbumId = {AlbumId} AND CartId = '{ShoppingCartId}' ";

                            SqlCommand command = new SqlCommand(sql6, connection);
                            command.ExecuteNonQuery();
                        }
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Retrieves every item in your shopping cart
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="CartItems">
        ///     Holds every item that is in your shopping cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that all items in your shopping cart have been retrieved
        /// </returns>

        public bool GetCartItems(HttpContext Context, List<Cart> CartItems)
        {
            var session = Context.Session;
            bool success = false;

            try
            {
                if (Connect())
                {
                    // Checks if you are already logged in so it knows if it is to find
                    //    your shopping cart ID or your username in the Cart database
                    if (session.GetString(AlreadyLogIn) == "True")
                    {
                        String sql1 =
                            $" SELECT RecordId, AlbumId, Count " +
                            $" From Cart " +
                            $" WHERE CartId = '{session.GetString(UserNameKey)}' ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql1, connection);
                        da.Fill(dt);

                        // Checks if you have any items in your shopping cart
                        if (dt.Rows.Count > 0)
                        {
                            Cart CartItem;

                            foreach (DataRow row in dt.Rows)
                            {
                                CartItem = new Cart()
                                {
                                    RecordId = (int)row["RecordId"],
                                    AlbumId = (int)row["AlbumId"],
                                    Count = (int)row["Count"],
                                    Album = new Album()
                                };

                                CartItems.Add(CartItem);
                            }

                            foreach (var item in CartItems)
                            {
                                string Title = "";
                                decimal Price = 0.00M;

                                String sql2 =
                                    $" SELECT Title, Price " +
                                    $" FROM Album " +
                                    $" WHERE AlbumId = '{item.AlbumId}' ";

                                DataTable dt2 = new DataTable();
                                SqlDataAdapter da2 = new SqlDataAdapter();
                                da2.SelectCommand = new SqlCommand(sql2, connection);
                                da2.Fill(dt2);

                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dt2.Rows)
                                    {
                                        Title = (string)row["Title"];
                                        Price = (decimal)row["Price"];
                                    }
                                }

                                item.Album.Title = Title;
                                item.Album.Price = Price;
                            }

                            success = true;
                        }
                        else
                        {
                            success = true;

                            Console.WriteLine("No records found.");
                        }
                    }
                    else
                    {
                        String sql1 =
                            $" SELECT RecordId, AlbumId, Count " +
                            $" From Cart " +
                            $" WHERE CartId = '{ShoppingCartId}' ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql1, connection);
                        da.Fill(dt);

                        // Checks if you have any items in your shopping cart
                        if (dt.Rows.Count > 0)
                        {
                            Cart CartItem;

                            foreach (DataRow row in dt.Rows)
                            {
                                CartItem = new Cart()
                                {
                                    RecordId = (int)row["RecordId"],
                                    AlbumId = (int)row["AlbumId"],
                                    Count = (int)row["Count"],
                                    Album = new Album()
                                };

                                CartItems.Add(CartItem);
                            }

                            foreach (var item in CartItems)
                            {
                                string Title = "";
                                decimal Price = 0.00M;

                                String sql2 =
                                    $" SELECT Title, Price " +
                                    $" FROM Album " +
                                    $" WHERE AlbumId = '{item.AlbumId}' ";

                                DataTable dt2 = new DataTable();
                                SqlDataAdapter da2 = new SqlDataAdapter();
                                da2.SelectCommand = new SqlCommand(sql2, connection);
                                da2.Fill(dt2);

                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dt2.Rows)
                                    {
                                        Title = (string)row["Title"];
                                        Price = (decimal)row["Price"];
                                    }
                                }

                                item.Album.Title = Title;
                                item.Album.Price = Price;
                            }

                            success = true;
                        }
                        else
                        {
                            success = true;

                            Console.WriteLine("No records found.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }           

            return success;
        }

        /// <summary>
        ///     Counts how many items are in your shopping cart
        /// </summary>
        /// 
        /// <param name="ShoppingCartItems">
        ///     Model that will hold the count of how many items you have in your cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means it has successfully retrieved how many items are in your cart
        /// </returns>

        public bool GetCartItemCount(ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems != null)
            {
                // Counts how many items are in your cart by looking at each item in your cart and how many
                //    of each item are in your cart
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    ShoppingCartItems.CartTotalItems = ShoppingCartItems.CartTotalItems + item.Count;
                }

                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Calculates the total cost of all the items in your cart
        /// </summary>
        /// 
        /// <param name="ShoppingCartItems">
        ///     Model that will hold the cost of all the items in your cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means it has successfully calculated the cost off all of the items in your cart
        /// </returns>

        public bool GetTotal(ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems  != null)
            {
                // Calculates the cost of each item in your cost
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    ShoppingCartItems.CartTotal = ShoppingCartItems.CartTotal + item.Count * item.Album.Price;
                }

                success = true;
            }
            else
            {
            }

            return success;
        }

        /// <summary>
        ///     When you are deleting an item from your cart, you use AJAX to retrieve the
        ///        title of the item you are deleting from your cart
        /// </summary>
        /// 
        /// <param name="RemoveItem">
        ///     A model that will hold the name of the album to indicate which item has
        ///        been deleted from your cart
        /// </param>
        /// 
        /// <param name="RecordId">
        ///     The ID of the record in the cart database that holds the deleted item
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that title of the item that has been deleted has been retrieved
        /// </returns>

        public bool GetAlbumTitleAJAX(ShoppingCartRemoveItem RemoveItem, int RecordId)
        {
            string AlbumTitle = "";
            int AlbumId = 0;
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT AlbumId " +
                        $" FROM Cart " +
                        $" WHERE RecordId = '{RecordId}' ";

                    SqlCommand cmd = new SqlCommand(sql, connection);
                    AlbumId = (int)cmd.ExecuteScalar();

                    String sql2 =
                        $" SELECT Title " +
                        $" FROM Album " +
                        $" WHERE AlbumId = {AlbumId} ";

                    SqlCommand cmd2 = new SqlCommand(sql2, connection);
                    AlbumTitle = (string)cmd2.ExecuteScalar();

                    RemoveItem.Message = AlbumTitle + " has been removed from your shopping cart.";

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Uses AJAX to remove the deleted item from your shopping cart and calculate
        ///        how many of that item is still in your cart
        /// </summary>
        /// 
        /// <param name="RemoveItem">
        ///     The model that holds the remaining count of the item after one of the item
        ///        has been deleted from your cart
        /// </param>
        /// 
        /// <param name="RecordId">
        ///     The record in the cart database that holds the item that has been deleted
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the item has been successfully deleted from your cart database
        /// </returns>

        public bool RemoveFromCartAJAX(ShoppingCartRemoveItem RemoveItem, int RecordId)
        {
            bool success = false;
            int ItemCount = 0;

            try
            {
                if (Connect())
                {
                    String sql1 =
                        $" SELECT Count " +
                        $" FROM Cart " +
                        $" WHERE RecordId = '{RecordId}' ";

                    SqlCommand cmd = new SqlCommand(sql1, connection);
                    ItemCount = (int)cmd.ExecuteScalar();

                    // Checks if deleting the item means you have no more of that item left
                    //    in your cart
                    if (ItemCount > 1)
                    {
                        ItemCount = ItemCount - 1;

                        String sql2 =
                            $" UPDATE Cart " +
                            $" SET Count = {ItemCount} " +
                            $" WHERE RecordId = '{RecordId}' ";

                        SqlCommand command = new SqlCommand(sql2, connection);
                        command.ExecuteNonQuery();

                        RemoveItem.ItemCount = ItemCount;
                    }
                    else
                    {
                        String sql3 =
                            $" Delete From Cart" +
                            $" WHERE RecordId = '{RecordId}' ";

                        SqlCommand command = new SqlCommand(sql3, connection);
                        command.ExecuteNonQuery();

                        RemoveItem.ItemCount = 0;
                    }

                    success = true;
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>
        ///     Calculates the cost of all the items in your cart using AJAX after one item has been deleted
        ///        from your cart
        /// </summary>
        /// 
        /// <param name="RemoveItem">
        ///     Model that holds the total cost of all the items iny our cart after an item has been deleted
        ///        from your cart
        /// </param>
        /// 
        /// <param name="ShoppingCartItems">
        ///     Model that holds all the items in your cart 
        /// </param>
        /// 
        /// <returns>
        ///     success=true means it has succesfully calculated the cost of all the items in your cart
        ///        after you have deleted an item from your cart
        /// </returns>

        public bool GetTotalAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                // Goes through each item in your cart after an item has been deleted to calculate the
                //    updated cost of all the items in your cart
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    RemoveItem.CartTotal = RemoveItem.CartTotal + item.Count * item.Album.Price;
                }

                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Counts the number of items in your cart using AJAX after you have deleted an item 
        ///        from your cart
        /// </summary>
        /// 
        /// <param name="RemoveItem">
        ///     Model that holds how many items are in your cart after you have deleted an item
        ///        from your cart
        /// </param>
        /// 
        /// <param name="ShoppingCartItems">
        ///     Model that holds all the items in your cart after you have deleted an item from
        ///        your cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means you have successfully counted how many items are in your cart
        ///        after you have deleted an item from your cart
        /// </returns>

        public bool GetCountAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                RemoveItem.CartCount = 0;

                // 
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    RemoveItem.CartCount = RemoveItem.CartCount + item.Count;
                }

                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Get all the items in your cart in AJAX for your header
        /// </summary>
        /// 
        /// <param name="Context">
        ///     Represents the entire context of an individual HTTP request and response
        /// </param>
        /// 
        /// <param name="CartItems">
        ///     A list to hold all the items in your cart
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that it has retrieved all the items from your cart
        /// </returns>

        public bool GetCartItemsHeaderAJAX(HttpContext Context, List<Cart> CartItems)
        {
            var session = Context.Session;
            bool success = false;

            try
            {
                if (Connect())
                {
                    // Checks if you are already logged in so it knows if it is to find
                    // your shopping cart ID or your username in the Cart database
                    if (session.GetString(AlreadyLogIn) == "True")
                    {
                        String sql1 =
                            $" SELECT Count " +
                            $" From Cart " +
                            $" WHERE CartId = '{session.GetString(UserNameKey)}' ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql1, connection);
                        da.Fill(dt);

                        // Checks if you have any items in your cart
                        if (dt.Rows.Count > 0)
                        {
                            Cart CartItem;

                            foreach (DataRow row in dt.Rows)
                            {
                                CartItem = new Cart()
                                {
                                    Count = (int)row["Count"],
                                };

                                CartItems.Add(CartItem);
                            }

                            success = true;
                        }
                        else
                        {
                            success = true;

                            Console.WriteLine("No records found.");
                        }                      
                    }
                    else
                    {
                        String sql1 =
                            $" SELECT RecordId, AlbumId, Count " +
                            $" From Cart " +
                            $" WHERE CartId = '{ShoppingCartId}' ";

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql1, connection);
                        da.Fill(dt);

                        // Checks if you have any items in your cart
                        if (dt.Rows.Count > 0)
                        {
                            Cart CartItem;

                            foreach (DataRow row in dt.Rows)
                            {
                                CartItem = new Cart()
                                {
                                    Count = (int)row["Count"],
                                };

                                CartItems.Add(CartItem);
                            }

                            success = true;
                        }
                        else
                        {
                            success = true;

                            Console.WriteLine("No records found.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to database");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return success;
        }

        /// <summary>  
        ///     Counts how many items are in your cart so it cna display in the header using AJAX 
        ///        how many items are in your cart
        /// </summary>
        /// 
        /// <param name="RemoveItem">
        ///     Model that holds how many items are in your cart
        /// </param>
        /// 
        /// <param name="ShoppingCartItems">
        ///     Model that holds every item in your cart and how many of each item is in your cart
        /// </param>
        /// 
        /// <returns>
        ///     succes=true means it has successfully caluclated how many items are in your cart
        /// </returns>

        public bool GetCountHeaderAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                RemoveItem.CartCount = 0;

                // Goes through each item in your cart to calculate how many items in total are in your cart
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    RemoveItem.CartCount = RemoveItem.CartCount + item.Count;
                }

                success = true;
            }
            else
            {
                RemoveItem.CartCount = 0;

                success = true;
            }

            return success;
        }
    }
}
