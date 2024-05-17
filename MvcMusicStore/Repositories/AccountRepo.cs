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
        /// <summary>
        ///    Checks if the username and passwrod you are logging in with matches
        ///       with what is in the database
        /// </summary>
        /// 
        /// <param name="LogOn">
        ///     Contains the username and password used to log on
        /// </param>
        /// 
        /// <returns>
        ///    success=true means that a matching record of the username and password 
        ///       was found in the database
        /// </returns>

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

        /// <summary>
        ///     Checks if the email you are registering with already exists or not in the database
        /// </summary>
        /// 
        /// <param name="Email">
        ///     Contains the email used to register for a new account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the email used to register for a new account 
        ///        does not exist in the database
        /// </returns>

        public bool RegisterEmail(String Email)
        {
            bool success = false;
            int count = 0;

            // If the email is sent as null, it converts it to an empty string
            Email = Email ?? "";

            try
            {
                if (Connect())
                {
                    // Checks in SQL if there is already a record of the email address
                    String sql =
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Email = '{Email}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);

                        // Checks to see that there is no record of this email address
                        // (count == 0 means the email address does not exist and so
                        //    you can create a new account with that email address)
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

        /// <summary>
        ///    Checks if the email is valid or not
        /// </summary>
        /// 
        /// <param name="Email">
        ///     Contains the email used to register for a new account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the email meets the requirements
        /// </returns>

        public bool CheckEmailValid(String Email)
        {
            bool success = false;

            // If the email is sent as null, it converts it to an empty string
            Email = Email ?? "";

            // Tells the regular expression criteria the email must meet
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Creates a new instance of the Regex class containing the regex pattern you want 
            //    to see if a string matches 
            Regex regex = new Regex(pattern);

            // Takes the actual string in Match to see if it matches the regex pattern matches
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

        /// <summary>
        ///     Counts how many occurrences a character is in a string
        /// </summary>
        /// 
        /// <param name="input">
        ///     The email undergoing regex testing
        /// </param>
        /// 
        /// <param name="target">
        ///     The selected character that is to be used to see how many times 
        ///        it exists in the email
        /// </param>
        /// 
        /// <returns>
        ///     How many times the selected character exists in the email
        /// </returns>
        
        private int CountOccurrences(string input, char target)
        {
            int count = 0;

            // Counts how many occurrences a character is in a string
            // Iterates through every character in the email seeing if it matches
            //    the selected character
            foreach (char c in input)
            {
                // Checks if the character currently being iterated on matches the 
                //    selected character
                if (c == target)
                {
                    // If matches the selected character count goes up by 1
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        ///     Checks to see if the username you are registering with already exists or not
        /// </summary>
        /// 
        /// <param name="UserName">
        ///     The UserName that the user submitted when creating a new account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the username is not been already taken and can be used
        /// </returns>
        
        public bool RegisterUserName(String UserName)
        {
            bool success = false;
            int count = 0;

            // If the username is sent as null, it converts it to an empty string
            UserName = UserName ?? "";

            try
            {
                if (Connect())
                {
                    // Checks in SQL if there is already a record of the username
                    String sql =
                        $" SELECT Count(*) as Count " +
                        $" From Account " +
                        $" WHERE Username = '{UserName}' ";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand(sql, connection);
                    da.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["Count"]);

                        // Checks to see if a record of this username already exists 
                        // (count == 0 means it does not exist and can be used)
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

        /// <summary>
        ///     Checks if the username is at least 6 characters long
        /// </summary>
        /// 
        /// <param name="UserName">
        ///     The UserName that the user submitted when creating a new account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the username meets the requiremnt of being at 
        ///         least 6 characters long and can be used
        /// </returns>

        public bool CheckUsernameLength(String UserName)
        {
            bool success = false;

            // If the username is sent as null, it converts it to an empty string
            UserName = UserName ?? "";

            // Checks if the username is at least 6 characters long
            if (UserName.Length >= 6)
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Checks if the password and confirm passwords match
        /// </summary>
        /// 
        /// <param name="Register">
        ///     Contains both the password and confirmpassword the user inputted 
        ///        when creating a new account to check if they match
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the password and confirmpassowrd match 
        /// </returns>
        
        public bool RegisterPasswords(Register Register)
        {
            bool success = false;

            // If the password is sent as null, it converts it to an empty string
            String Password = Register.Password ?? "";

            // If the confirm password is sent as null, it converts it to an empty string
            String ConfirmPassword = Register.ConfirmPassword ?? "";

            // Checks if the password and confirm password match
            if (Register.Password == Register.ConfirmPassword)
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Checks if the password is at least 6 characters long
        /// </summary>
        /// 
        /// <param name="Password">
        ///     Contains the password the user inputted when creating a new account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the password meets the requirement of being at leaat
        ///         6 characters long and can be used
        /// </returns>
        
        public bool CheckPasswordLength(String Password)
        {
            bool success = false;

            // If the password is sent as null, it converts it to an empty string
            Password = Password ?? "";

            // Checks if the password is at least 6 characters long
            if (Password.Length >= 6)
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Insert into SQL the newly created account's username, password, and email
        /// </summary>
        /// 
        /// <param name="Register">
        ///     Contains the newly created account's username, password, and email that 
        ///        will be inserted into the database of users
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the new account and its user and login information
        ///         have been successfully inserted into the database of users
        /// </returns>

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

        /// <summary>
        ///     Used to check if the username and current password match to verify user
        /// </summary>
        /// 
        /// <param name="ChangePassword">
        ///     Contains the username and the current password to verify the user who 
        ///        wants to change the account password
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the user has been verified and can change the account password
        /// </returns>
        
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
                        $" WHERE Username = '{ChangePassword.UserName}' " +
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

        /// <summary>
        ///     Checks if the new password and confirm password match
        /// </summary>
        /// 
        /// <param name="ChangePassword">
        ///     Contians the new password and confirm password to see if they match
        /// </param>
        /// 
        /// <returns>
        ///     success=true means the new password and confirm password match and the
        ///        new password can be used for the account
        /// </returns>
        
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

        /// <summary>
        ///     Insert into SQL the newly created account's username, password, and email
        /// </summary>
        /// 
        /// <param name="ChangePassword">
        ///     Contains the user account information and the new password for the account
        /// </param>
        /// 
        /// <returns>
        ///     success=true means that the user account has been updated with the new password
        ///        in the database
        /// </returns>

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