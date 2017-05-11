

using MySql.Data.MySqlClient;
using System;

namespace TempServer
{
    class DBController
    {

        MySqlConnection connection;

        public DBController(string d_base, string host, string user_id, string password)
        {
            // TODO: Read from config file
            string connect_line = "server="+host+";uid="+user_id+";" +
                        "pwd="+password+";database="+d_base+";";

            connection = new MySqlConnection(connect_line);
            try {
                connection.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        ~DBController()
        {
            connection.Close();
        }
        

        public bool CheckAuth(string username, string passwordHash)
        {
            string verify_command_text = "SELECT password FROM users WHERE username='" + username+ "'";
            MySqlCommand command = new MySqlCommand(verify_command_text, connection);
            //MySqlDataReader passwordReader = command.ExecuteReader();
            ////if (!passwordReader.Read()) string;
            //passwordReader.Read();

            //string password = passwordReader.GetString(0);
            object recived = command.ExecuteScalar();
            if (recived == null)
            {
                return false;
            }
            string password = recived.ToString();

            //return password.GetHashCode().ToString() == passwordHash;
            return password == passwordHash;
        }

        public bool TryRegister(string username, string passwordHash)
        {
            string check_name_command_text = "SELECT username FROM users WHERE username='" + username + "'";
            MySqlCommand command = new MySqlCommand(check_name_command_text, connection);
            // what's back?!
            if (command.ExecuteScalar() != null)
            {
                return false;
            }
            string create_user_command_text = "INSERT INTO `users`(`username`, `password`) VALUES ('" +
                username + "', '" + passwordHash + "')";
            MySqlCommand insertion = new MySqlCommand(create_user_command_text, connection);
            insertion.ExecuteNonQuery();
            return true;
        }


    }
}
