

using MySql.Data.MySqlClient;

namespace TempServer
{
    class DBController
    {
        string connect_line;

        public DBController(string d_base, string host, string user_id, string password)
        {
            connect_line = "Database="+d_base+";Data Source="+host+
                ";User Id="+user_id+";Password="+password;
            connect_line = "server="+host+";uid="+user_id+";" +
                        "pwd="+password+";database="+d_base+";";
        }
        

        public bool CheckAuth(string username, string passwordHash)
        {

            MySqlConnection connection = new MySqlConnection(connect_line);
            connection.Open();
            //TODO: debug
            string verify_command_text = "SELECT password FROM users WHERE username='" + username+ "'";
            MySqlCommand command = new MySqlCommand(verify_command_text, connection);
            MySqlDataReader passwordReader = command.ExecuteReader();
            //if (!passwordReader.Read()) string;
            passwordReader.Read();

            string password = passwordReader.GetString(0);

            connection.Close();
            return password.GetHashCode().ToString() == passwordHash;
        }
    }
}
