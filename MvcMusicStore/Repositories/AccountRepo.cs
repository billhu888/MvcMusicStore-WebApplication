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
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MvcMusicStore.Repositories
{
    class AccountRepo : BaseRepo
    {
        public bool LogOn(LogOn LogOn)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Username = '{LogOn.UserName}' " +
                        $"    AND Password = '{LogOn.Password}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);

                        // Checks to see if there is a record of this
                        // username and password (1 means yes so it means
                        // that you have been authenticated and can log in)
                        if (count == 1)
                        {
                            success = true;
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

        public bool RegisterEmail(Register Register)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Email = '{Register.Email}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);

                        // Checks to see if a record of this email
                        // already exists (0 means it does not exist and so
                        // you can create a new account with that email)
                        if (count == 0)
                        {
                            success = true;
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

        public bool RegisterUserName(Register Register)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Username = '{Register.UserName}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);


                        // Checks to see if a record of this username
                        // already exists (0 means it does not exist
                        // and can be created)
                        if (count == 0)
                        {
                            success = true;
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

        public bool RegisterPasswords(Register Register)
        {
            bool success = false;
            
            if (Register.Password == Register.ConfirmPassword)
            {
                success = true;
            }

            return success;
        }

        public bool AddAccount(Register Register)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql =
                        $" INSERT INTO Account (Username, Password, Email) " +
                        $" VALUES ('{Register.UserName}', '{Register.Password}', " +
                        $"   '{Register.Email}') ";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

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

        public bool ChangePasswordConfirmUserPassword(ChangePassword ChangePassword)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    String sql = 
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Email = '{ChangePassword.UserName}' " +
                        $"    AND Password = '{ChangePassword.CurrentPassword}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);

                        // Checks to see if a record of this username and password
                        // already exists (1 means it exists and so password
                        // can be changed for that username)
                        if (count == 1)
                        {
                            success = true;
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

        public bool ChangePasswordsNewPassword(ChangePassword ChangePassword)
        {
            bool success = false;
            
            if (ChangePassword.NewPassword == ChangePassword.ConfirmNewPassword)
            {
                success = true;
            }

            return success;
        }

        public bool ChangePasswordUpdatePassword(ChangePassword ChangePassword)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    String sql = 
                        $" INSERT INTO Account (Password) " +
                        $" VALUES ('{ChangePassword.NewPassword}') ";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

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
    }
}