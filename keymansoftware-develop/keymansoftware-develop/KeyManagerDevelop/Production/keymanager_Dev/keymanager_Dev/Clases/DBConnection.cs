using Npgsql;
using System;

namespace keymanager_Dev.Clases
{
    internal class DBConnection
    {
        private static string connectionString = "Host=localhost;Port=5432;Database=keymandb;Username=postgres;Password=123456";
        private static DBConnection instance;
        private static NpgsqlConnection con;

        private DBConnection()
        {
        }

        public static DBConnection getInstance()
        {
            if (instance == null)
            {
                instance = new DBConnection();
            }
            return instance;
        }

        public NpgsqlConnection getConnection()
        {
            if (con == null)
            {
                try
                {
                    con = new NpgsqlConnection(connectionString);
                }
                catch (NpgsqlException e)
                {
                    Console.WriteLine("Error en la getConnection: " + e.Message);
                }
            }
            return con;
        }
    }
}
