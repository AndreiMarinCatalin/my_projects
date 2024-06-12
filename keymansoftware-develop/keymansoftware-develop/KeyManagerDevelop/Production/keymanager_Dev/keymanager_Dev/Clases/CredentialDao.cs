using Microsoft.VisualBasic.ApplicationServices;
using Npgsql;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace keymanager_Dev.Clases
{
    public class CredentialDao
    {
        private DBConnection dbConnect;
        private NpgsqlConnection con;

        public CredentialDao() 
        {
            dbConnect = DBConnection.getInstance();
        }

        public List<Credential> getCredentialsByUserId(int id)
        {
            List<Credential> credentials = new List<Credential>();

            String query = "Select id_credential, name, link, username, password, description, expiration_date from credential where id_user = @id Order by id_credential desc";
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
                                credentials = new List<Credential>();
                                while (reader.Read())
                                {
                                    int credentialId = Convert.ToInt32(reader[0]);
                                    string name = reader[1].ToString();
                                    string link = reader[2].ToString();
                                    string username = reader[3].ToString();
                                    string password = reader[4].ToString();
                                    string description = reader[5].ToString();
                                    DateTime expirationDate = Convert.ToDateTime(reader[6]);

                                    credentials.Add(new Credential(credentialId, name, link, username, password, description, expirationDate));
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
                    MessageBox.Show("Error CredentialDao_getCredentialsByUserId: " + error1.Message);
                }
            }

            return credentials;
        }


        /// <summary>
        /// Gets pass from credentia by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getSingleCredentialById(int id)
        {
            string credentials = null;

            String query = "Select password from credential where id_credential= @id";
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

                                while (reader.Read())
                                {
                                    credentials = reader[1].ToString();

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

            return credentials;
        }

        public int deleteCredential(int idCredential)
        {
            int success = -1;
            String query = "DELETE FROM credential WHERE id_credential = @id_credential";  // Nota el cambio de "user" a "users"

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
                        cmd.Parameters.AddWithValue("@id_credential", idCredential); // Asume que user.Username es una propiedad del objeto "user"

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

        public int saveCredential(Credential credential)
        {
            int success = 0;
            String query = "INSERT INTO credential (id_user, name, username, password, expiration_date) VALUES (@id_user, @name, @username, @password, @expdate)";

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
                        cmd.Parameters.AddWithValue("@name", credential.Name);
                        cmd.Parameters.AddWithValue("@username", credential.Username);
                        cmd.Parameters.AddWithValue("@password", credential.Password);
                        cmd.Parameters.AddWithValue("@expdate", credential.expiration_data);

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

        /*
         * 
         * 
            INSERT INTO CREDENTIAL (id_user, name, link, username, password, description, expiration_date)
            VALUES (4, 'Ejemplo', 'https://www.ejemplo.com', 'usuario_ejemplo', 'contraseña123', 'Descripción de ejemplo', '2023-12-31');
         * 
         */


    public int changePasswordCredential(int idCredential, string password, DateTime dateTime)
    {
        int success = 0;
        String query = "UPDATE credential SET password = @password, expiration_date = @dateTime  WHERE id_credential = @credential_id";

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
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@dateTime", dateTime);
                    cmd.Parameters.AddWithValue("@credential_id", idCredential);

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

        public int changeCredentialById(Credential credential)
        {
            int success = 0;
            String query = "UPDATE credential SET name = @title, username = @username, password = @password, expiration_date = @expirationDate WHERE id_credential = @credential_id";

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
                        cmd.Parameters.AddWithValue("@title", credential.Name);
                        cmd.Parameters.AddWithValue("@username", credential.Username);
                        cmd.Parameters.AddWithValue("@password", credential.Password);
                        cmd.Parameters.AddWithValue("@expirationDate", credential.expiration_data);
                        cmd.Parameters.AddWithValue("@credential_id", credential.id_user); //in this case is Credential's id

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

        public bool changeMasterKey(string Key, string masterKey, List<Credential> credentialList)
        {
            bool success = false;

            
            string newkeyHash = Key.Substring(0, Math.Min(Key.Length, 16));
            string newKey = Encryptor.Encrypt(Key, Encryptor.KEY);

            string oldKey = masterKey.Substring(0, Math.Min(Key.Length, 16));

            con = dbConnect.getConnection();
            
            if (con != null)
            {
                try
                {
                    if (con.State == System.Data.ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            
                            // Obtener y desencriptar las contraseñas, reencriptarlas y guardarlas en la base
                            foreach (var credential in credentialList)
                            {
                                string pass = Encryptor.Decrypt(credential.Password, oldKey);
                                credential.Password = Encryptor.Encrypt(pass, newkeyHash);
                                _= changePasswordCredential(credential.Id,credential.Password, credential.expiration_data);
                            }
                            
                            //guardar la nueva master Key
                            UserDao dao = new UserDao();

                            dao.updateMasterKey(newKey);
                            

                            // Si todo es exitoso, confirmar la transacción
                            tran.Commit();
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            // Si hay un error, revertir la transacción
                            tran.Rollback();
                            //Console.WriteLine($"Error: {ex.Message}");
                        }
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

        public int removeAllCredentialByUserId(int id_user)
        {
            int success = 0;

            String query = "Delete from credential where id_user = @id_user;";

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
    }
}
