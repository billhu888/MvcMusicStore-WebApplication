using Microsoft.Data.SqlClient;
using System.Linq;
using System.Data;

namespace MvcMusicStore.Repositories
{
    abstract class BaseRepo
    {
        protected bool isConnected = false;
        protected SqlConnection connection;

        public BaseRepo()
        {
            var objBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

            IConfiguration conManager = objBuilder.Build();
            string connectionString = conManager.GetConnectionString("MvcMusicStore");

            try
            {
                connection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected bool Connect()
        {
            try
            {
                if (!isConnected)
                {
                    connection.Open();
                    isConnected = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                isConnected = false;
            }

            return isConnected;
        }
    }
}