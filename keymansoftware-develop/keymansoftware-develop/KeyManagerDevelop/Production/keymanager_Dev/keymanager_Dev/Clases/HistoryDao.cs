using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keymanager_Dev.Clases
{
    public class HistoryDao
    {
        private DBConnection dbConnect;
        private NpgsqlConnection con;
        public HistoryDao() 
        {
            dbConnect = DBConnection.getInstance();
        }

        /// <summary>
        /// Gets all credentials by his id_user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<History> getCredentialsByUserId(int id)
        {
            List<History> history = null;

            String query = "Select  * from history where id_user= @id";
            DataSet dataset = new DataSet();

            con = dbConnect.getConnection();
            if (con != null)
            {

                try
                {
                    if (con.State == System.Data.ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                history = new List<History>();
                                while (reader.Read())
                                {
                                    history.Add(new History(Convert.ToInt32(reader[0]), Convert.ToInt32(reader[1]), reader[2].ToString()));

                                }
                            }
                            else
                            {
                                Console.WriteLine("No rows found in table");
                            }
                        }
                    }
                }
                catch (NpgsqlException error1)
                {
                    throw;
                }

            }

            return history;
        }
        

        public int saveCredential(Credential credential)
        {
            int success = 0;
            String query = "INSERT INTO history (id_user, expired_password) VALUES (@id_user, @expired_password)";

            con = dbConnect.getConnection();

            if (con != null)
            {
                try
                {
                    if (con.State == System.Data.ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_user", credential.id_user);
                        cmd.Parameters.AddWithValue("@expired_password", credential.Password);
                        

                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {

                    throw;
                }
            }
            return success;
        }

        public int deleteHistory(int id_user)
        {
            int success = 0;
            String query = "Delete from history where id_user = @id_user;";

            con = dbConnect.getConnection();
            if (con != null)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_user", id_user);
                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {
                    throw;
                }
            }
            return success;
        }

        public int resetAutoIncrementSequenceHistory()
        {
            int success = 0;
            String query = "SELECT setval(pg_get_serial_sequence('history', 'id_history'), coalesce(max(id_history), 0) + 1, false) FROM history;";
            con = dbConnect.getConnection();
            if (con != null)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {
                    throw;
                }
            }

            return success;
        }

    }
}
