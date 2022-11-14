using System.Diagnostics;
using System.Data;
using MySql.Data.MySqlClient;
using API.Models;

namespace API.DAL
{
    public class DBManager
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        //constructor setting connectionstring to databases
        public DBManager(IConfiguration _configuration)
        {
            configuration = _configuration;
            connectionString = configuration.GetConnectionString("DBContext");
        }




        #region UserAdministration


        internal void AddUser(User user)
        {
            MySqlConnection connection = new(connectionString);

            connection.Open();

            try
            {
                MySqlCommand cmd = new()
                {
                    CommandText = "addUser",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                cmd.Parameters.AddWithValue("@_userName", user.UserName);
                cmd.Parameters["@_userName"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@_passwordHash", user.PasswordHash);
                cmd.Parameters["@_passwordHash"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@_passwordSalt", user.PasswordSalt);
                cmd.Parameters["@_passwordSalt"].Direction = ParameterDirection.Input;

                //Sending data to DB
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }




        internal void UpdateUserSalt(User user)
        {
            MySqlConnection connection = new(connectionString);

            connection.Open();

            try
            {
                MySqlCommand cmd = new()
                {
                    CommandText = "updateUser",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                cmd.Parameters.AddWithValue("@_userName", user.UserName);
                cmd.Parameters["@_userName"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@_passwordHash", user.PasswordHash);
                cmd.Parameters["@_passwordHash"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@_passwordSalt", user.PasswordSalt);
                cmd.Parameters["@_passwordSalt"].Direction = ParameterDirection.Input;

                //Sending data to DB
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }




        internal User GetUser(string? userName)
        {
            MySqlConnection connection = new(connectionString);

            try
            {
                MySqlCommand cmd = new()
                {
                    CommandText = "getUser",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                connection.Open();

                cmd.Parameters.AddWithValue("@_userName", userName);
                cmd.Parameters["@_userName"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();
                User? dbUser = new();

                while (reader.Read())
                {
                    dbUser.UserId = (int)reader["userId"];
                    dbUser.UserName = reader["userName"].ToString();
                    dbUser.PasswordHash = (byte[])reader["passwordHash"];
                    dbUser.PasswordSalt = (byte[])reader["passwordSalt"];
                }
                cmd.Parameters.Clear();
                return dbUser;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }


        #endregion

   
    }
}
