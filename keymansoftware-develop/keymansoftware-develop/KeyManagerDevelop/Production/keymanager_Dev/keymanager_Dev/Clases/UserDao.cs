using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace keymanager_Dev.Clases
{
    public class UserDao
    {
        private DBConnection dbConnect;
        private NpgsqlConnection con;

        public UserDao()
        {
            dbConnect = DBConnection.getInstance();
        }

        public List<User> getAll()
        {
            List<User> users = null;

            String query = "Select * from \"user\"";
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
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                users = new List<User>();
                                while (reader.Read())
                                {
                                    users.Add(new User(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
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

            return users;

        }

        /// <summary>
        /// Gets the id of the user by his username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int getUserIdByUsername(string username)
        {
            int id_user = -1;

            String query = "Select id_user from \"user\" where username = @username";
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
                        cmd.Parameters.AddWithValue("@username", username);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    id_user = Convert.ToInt32(reader[0]);

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



            return id_user;
        }

        /// <summary>
        /// Gets the id of the user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> User or null</returns>
        public User getUserByUsername(string username)
        {
            User user = null;

            String query = "Select * from \"user\" where username = @username";
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
                        cmd.Parameters.AddWithValue("@username", username);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    user = new User(reader[0].ToString(), reader[1].ToString(), reader[2].ToString());

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



            return user;
        }

        /// <summary>
        /// Gets the encrypted masterkey of the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>masterkey o empty string</returns>
        public string getUserKeyByUsername(string username)
        {
            string pass = "";

            String query = "Select masterkey from \"user\" where username = @username";
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
                        cmd.Parameters.AddWithValue("@username", username);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    pass = reader[0].ToString();

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
            return pass;
        }


        public string getUserMailByUsername(string username)
        {
            string mail = "";

            String query = "Select email from \"user\" where username = @username";
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
                        cmd.Parameters.AddWithValue("@username", username);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    mail = reader[0].ToString();

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
            return mail;
        }


        /// <summary>
        /// Saves the new User in database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int saveUser(User user) 
        {
            int success = 0;
            String query = "INSERT INTO \"user\" (username, email, masterKey) VALUES (@username, @email, @masterKey)";

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
                        cmd.Parameters.AddWithValue("@username", user.Username); // Asume que user.Username es una propiedad del objeto "user"
                        cmd.Parameters.AddWithValue("@email", user.Email); // Asume que user.Email es una propiedad del objeto "user"
                        cmd.Parameters.AddWithValue("@masterKey", user.MasterKey); // Asume que user.MasterKey es una propiedad del objeto "user"

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

        /// <summary>
        /// Updates current masterKey of the user
        /// </summary>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public int updateMasterKey(string newPass)
        {
            int success = 0;
            String query = "UPDATE \"user\" SET masterKey = @masterKey WHERE username = @username";

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
                        cmd.Parameters.AddWithValue("@username", Environment.UserName); // Asume que user.Username es una propiedad del objeto "user"
                        cmd.Parameters.AddWithValue("@masterKey", newPass); // Asume que user.MasterKey es una propiedad del objeto "user"

                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {
                    // Puedes manejar el error de forma adecuada aquí, por ahora solo re-lanzo la excepción.
                    throw;
                }
            }
            return success;
        }

        //TO TEST YET
        public int deleteUser(User user)
        {
            int success = 0;
            String query = "DELETE FROM \"user\" WHERE username = @username";  // Nota el cambio de "user" a "users"

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
                        cmd.Parameters.AddWithValue("@username", user.Username); // Asume que user.Username es una propiedad del objeto "user"

                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {
                    // Puedes manejar el error de forma adecuada aquí, por ahora solo re-lanzo la excepción.
                    throw;
                }
            }
            return success;
        }

        public int changeMailByUser(string username, string email)
        {
            int success = 0;
            String query = "UPDATE \"user\" SET email = @email WHERE username = @username";

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
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@username", username);
                        success = cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException error1)
                {
                    // Handle the exception
                    throw;
                }
            }
            return success;
        }
    }
}
