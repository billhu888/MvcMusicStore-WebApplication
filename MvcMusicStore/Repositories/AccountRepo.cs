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
using System.Text.RegularExpressions;

namespace MvcMusicStore.Repositories
{
    class AccountRepo : BaseRepo
    {
        // Checks if the username and passwrod you are logging in with matches
        public bool LogOn(LogOn LogOn)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    // Checks in SQL if there is a record of the username and password
                    // that you entered
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

                        // count == 1 means there is a record of this username and password
                        // meaning that you have been authenticated and can log into your account
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

        // Checks to see if the email you are registering with already exists or not
        public bool RegisterEmail(Register Register)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    // Checks in SQL if there is already a record of the email address
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

                        // Checks to see that there is no record of this email address
                        // (count == 0 means the email address does not exist and so
                        // you can create a new account with that email address)
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

        // Checks if the email is valid or not
        public bool CheckEmailValid(String Email)
        {
            bool success = false;

            Email = Email ?? "";

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(Email);

            if (match.Success)
            {
                // Additional custom conditions to validate email structure
                if (CountOccurrences(Email, '@') == 1 &&
                    CountOccurrences(Email, '.') == 1 &&
                    Email.IndexOf('@') > 0 && // At least one character before @
                    Email.IndexOf('.') > Email.IndexOf('@') + 1 && // At least one character after @
                    Email.Length > Email.IndexOf('.') + 1) // At least one character after .
                {
                    success = true;
                }
            }

            return success;
        }

        // Counts how many occurrences each character is in a string
        private int CountOccurrences(string input, char target)
        {
            int count = 0;
            foreach (char c in input)
            {
                if (c == target)
                {
                    count++;
                }
            }
            return count;
        }

        // Checks to see if the username you are registering with already exists or not
        public bool RegisterUserName(Register Register)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    // Checks in SQL if there is already a record of the username
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

                        // Checks to see if a record of this username already exists 
                        // (count == 00 means it does not exist and can be used)
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

        // Checks if the username is at least 6 characters long
        public bool CheckUsernameLength(String Username)
        {
            bool success = false;

            Username = Username ?? "";

            if (Username.Length >= 6)
            {
                success = true;
            }

            return success;
        }

        // Checks if the password and confirm passwords match
        public bool RegisterPasswords(Register Register)
        {
            bool success = false;

            // Checks if the password and confirm passwords match
            if (Register.Password == Register.ConfirmPassword)
            {
                success = true;
            }

            return success;
        }

        // Checks if the password is at least 6 characters long
        public bool CheckPasswordLength(String Password)
        {
            bool success = false;

            Password = Password ?? "";

            if (Password.Length >= 6)
            {
                success = true;
            }

            return success;
        }

        // Insert into SQL the newly created account's username, password, and email
        public bool AddAccount(Register Register)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    // Used to insert into SQL the newly created account's username, password, and email
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

        // Used to check if the username and current password match to verify user
        public bool ChangePasswordConfirmUserPassword(ChangePassword ChangePassword)
        {
            bool success = false;
            int count = 0;

            try
            {
                if (Connect())
                {
                    // Used to check in SQL if the username and current password match to verify user
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

                        // Checks to see if a record of this username and password already exists
                        // (count == 1 means record exists and so password can be changed for that username)
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

        // Checks if the password and confirm passwords match
        public bool ChangePasswordsNewPassword(ChangePassword ChangePassword)
        {
            bool success = false;

            // Checks if the password and confirm passwords match
            if (ChangePassword.NewPassword == ChangePassword.ConfirmNewPassword)
            {
                success = true;
            }

            return success;
        }

        // Insert into SQL the newly created account's username, password, and email
        public bool ChangePasswordUpdatePassword(ChangePassword ChangePassword)
        {
            bool success = false;

            try
            {
                if (Connect())
                {
                    // Used to insert into SQL the newly created account's username, password, and email
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