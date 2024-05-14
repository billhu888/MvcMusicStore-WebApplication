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

        // Default Constructor
        public ShoppingCartRepo() 
        { }

        // Constructor to get the Cart ID
        public ShoppingCartRepo(HttpContext context)
        {
            ShoppingCartId = GetCartId(context);
        }

        // Constructor if you are logging in and you have a
        // cart and its ID that have not been checked out yet
        public ShoppingCartRepo(HttpContext context, String UserName)
        {
            UserNameOrder = GetCartId(context, UserName);
        }

        // Get the shopping cart ID for the cart
        public String GetCartId(HttpContext Context)
        {
            var session = Context.Session;

            // Checks to see if the session already has a Cart ID
            // If the session doesn't yet have a Cart ID it creates one
            // If the session already has a Cart ID it retrieves it
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

        // The Cart ID and its value is preserved after it is created
        public String GetCartId(HttpContext Context, String UserName)
        {
            var session = Context.Session;

            if (!session.Keys.Contains(UserNameKey))
            {
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    session.SetString(UserNameKey, UserName);
                }
                else
                {
                    session.SetString(UserNameKey, UserName);
                }
            }

            return session.GetString(UserNameKey);
        }

        public void ConfirmIndicatorFromCheckout(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(ComeFromCheckout, "True");
        }

        public string CheckFromCheckoutIndicator(HttpContext Context)
        {
            var session = Context.Session;       

            return session.GetString(ComeFromCheckout);
        }

        public void CheckFromCheckoutIndicatorReturnFalse(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(ComeFromCheckout, "True");
        }

        public void ConfirmLoggedOn(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(AlreadyLogIn, "True");
        }

        // Checks if you have already logged on 
        public string CheckIfLoggedOn(HttpContext Context)
        {
            var session = Context.Session;

            return session.GetString(AlreadyLogIn);
        }

        public void ConfirmLogOnFromHeader(HttpContext Context)
        {
            var session = Context.Session;

            session.SetString(LogOnFromHeader, "True");
        }

        public string CheckLogOnFromHeader(HttpContext Context)
        {
            var session = Context.Session;

            return session.GetString(LogOnFromHeader);
        }

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

        public void MigrateCart(HttpContext Context, Order order)
        {
            try
            {
                if (Connect())
                {
                    var session = Context.Session;
                    decimal TotalCost = 0.00M;

                    order.Username = session.GetString(UserNameKey);
                    
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

                    foreach (var item in order.OrderDetails)
                    {
                        TotalCost = TotalCost + item.Quantity * item.UnitPrice;
                    }

                    String sql2 =
                        $" SELECT Count(*) " +
                        $" FROM Orders ";

                    SqlCommand cmd = new SqlCommand(sql2, connection);
                    int rowCount = (int)cmd.ExecuteScalar();

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

                    order.Total = TotalCost;
                    DateTime dateTime = DateTime.Now;

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

                    if (session.GetString(AlreadyLogIn) == "True") 
                    {
                        String sql3 =
                            $" SELECT Count(*) AS Records " +
                            $" FROM Cart " +
                            $" WHERE AlbumId = {AlbumId} AND " +
                            $"    CartId = '{session.GetString(UserNameKey)}' ";

                        SqlCommand cmd2 = new SqlCommand(sql3, connection);
                        int rowCount2 = (int)cmd2.ExecuteScalar();

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

        public bool GetCartItems(HttpContext Context, List<Cart> CartItems)
        {
            var session = Context.Session;
            bool success = false;

            try
            {
                if (Connect())
                {
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

        public bool GetCartItemCount(ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems != null)
            {
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    ShoppingCartItems.CartTotalItems = ShoppingCartItems.CartTotalItems + item.Count;
                }
            }

            return success;
        }

        public bool GetTotal(ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems  != null)
            {
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    ShoppingCartItems.CartTotal = ShoppingCartItems.CartTotal + item.Count * item.Album.Price;
                }

                success = true;
            }
            else
            {}

            return success;
        }

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

        public bool GetTotalAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                foreach (var item in ShoppingCartItems.CartItems)
                {
                    RemoveItem.CartTotal = RemoveItem.CartTotal + item.Count * item.Album.Price;
                }

                success = true;
            }
            else
            {
            }

            return success;
        }

        public bool GetCountAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                RemoveItem.CartCount = 0;

                foreach (var item in ShoppingCartItems.CartItems)
                {
                    RemoveItem.CartCount = RemoveItem.CartCount + item.Count;
                }

                success = true;
            }
            else
            {}

            return success;
        }

        public bool GetCartItemsHeaderAJAX(HttpContext Context, List<Cart> CartItems)
        {
            var session = Context.Session;
            bool success = false;

            try
            {
                if (Connect())
                {
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

        public bool GetCountHeaderAJAX(ShoppingCartRemoveItem RemoveItem, ShoppingCartItems ShoppingCartItems)
        {
            bool success = false;

            if (ShoppingCartItems.CartItems != null)
            {
                RemoveItem.CartCount = 0;

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