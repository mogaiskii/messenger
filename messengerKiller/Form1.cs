using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace messengerKiller
{
    public partial class Form1 : Form
    {
        const int PORT = 8010;
        const string address = "127.0.0.1"; // TODO: debug

        TcpClient client = null;
        NetworkStream stream = null;
        bool listen = false;

        string username = "";

        public Form1()
        {
            InitializeComponent();

           
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (username.Length == 0)
            {
                username = nameTextBox.Text;
                nameTextBox.ReadOnly = true;
                nameTextBox.Enabled = false;
                sendButton.Enabled = true;
                loginButton.Text = "Logout";

                Login();
            }
            else
            {
                Logout();

                username = "";
                nameTextBox.ReadOnly = false;
                nameTextBox.Enabled = true;
                sendButton.Enabled = false;
                loginButton.Text = "Login";
            }
        }

        private void Login()
        {
            client = new TcpClient(address, PORT);
            stream = client.GetStream();

            listen = true;
            Task reciveTask = new Task(Recive);
            reciveTask.Start();
        }

        private void Logout()
        {
            listen = false;
            client.Close();
            stream.Close();
        }

        private void Recive()
        {
            while (listen)
            {
                byte[] data = new byte[64];
                StringBuilder strBuild = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    strBuild.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (stream.DataAvailable);
                string message = "\n" + strBuild.ToString();
                chatTextBox.Text += message;
                
            }
            if (!listen)
            {
                return;
            }
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if(client != null)  
                Logout();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string message = messageBox.Text;
            message = username + " " + message;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

            messageBox.Text = "";
        }
    }
}
