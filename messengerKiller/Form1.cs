using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace messengerKiller
{
    public partial class Form1 : Form
    {
        const int NAME_LEN = 32;

        const int PORT = 8010;
        const string address = "127.0.0.1"; // TODO: debug

        TcpClient client = null;
        NetworkStream stream = null;
        bool listen = false;
        delegate void ChatWriteDelegate(string message);
        delegate void ChangeGuiDelegate();

        string username = "";

        public Form1()
        {
            InitializeComponent();

           
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (username.Length == 0)
            {
                if (nameTextBox.Text.Length <= NAME_LEN && nameTextBox.Text.Length > 0
                    && !nameTextBox.Text.Contains("%") && nameTextBox.Text[0]!=' '
                    && nameTextBox.Text != "SERVER" )
                {
                    

                    Login();
                }
                else
                {
                    chatTextBox.Text += "\n!-- Name is incorrect !";
                }
            }
            else
            {
                Logout();

                
            }
        }

        private void Login()
        {
            username = nameTextBox.Text;
            nameTextBox.ReadOnly = true;
            nameTextBox.Enabled = false;
            sendButton.Enabled = true;
            loginButton.Text = "Logout";
            chatTextBox.BackColor = System.Drawing.Color.LightGreen;
            
            try
            {
                client = new TcpClient(address, PORT);
                stream = client.GetStream();

                listen = true;
                Task reciveTask = new Task(Recive);
                reciveTask.Start();
                chatTextBox.Text = "";

                Send("SERVER", "AUTH" + username);
            }
            catch(Exception ex)
            {
                chatTextBox.Text += '\n'+ ex.Message;
                Logout();
            }
        }

        private void Logout()
        {
            listen = false;
            try
            {
                Send("SERVER", "EXIT");
            }
            catch
            {

            }
            if (client != null)
                client.Close();
            if (stream != null)
                stream.Close();

            username = "";
            if (nameTextBox.InvokeRequired)
            {
                ChangeGuiDelegate del = delegate ()
                {
                    nameTextBox.ReadOnly = false;
                    nameTextBox.Enabled = true;
                    sendButton.Enabled = false;
                    loginButton.Text = "Login";
                    chatTextBox.BackColor = System.Drawing.Color.LightGreen;
                };
                nameTextBox.Invoke(del);
            }
            else
            {
                nameTextBox.ReadOnly = false;
                nameTextBox.Enabled = true;
                sendButton.Enabled = false;
                loginButton.Text = "Login";
                chatTextBox.BackColor = System.Drawing.Color.LightGreen;
            }
        }

        private void Recive()
        {
            try
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
                    ChatWriteDelegate del = delegate (string mes) { chatTextBox.Text += message; };
                    chatTextBox.Invoke(del, message);
                    //chatTextBox.Text += message;

                }
            }
            catch(Exception ex)
            {
                listen = false;
                Logout();
                ChatWriteDelegate del = delegate (string mes) { chatTextBox.Text += ex.Message; };
                chatTextBox.Invoke(del, ex.Message);
            }
            if (!listen)
            {
                return;
            }
        }

        private void Send(string send_to, string message)
        {
            if (send_to.Length < NAME_LEN)
            {
                send_to += string.Concat(Enumerable.Range(0, (NAME_LEN - send_to.Length)).Select(i => "%"));
            }
            message = send_to +  message;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void Form1_Closing(object sender, EventArgs e)
        { 
            Logout();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string message = messageBox.Text;

            string send_to = message.Substring(1, message.IndexOf('%',1));

            message = message.Substring(message.IndexOf('%',1)+1);

            //////
            try
            {
                Send(send_to, message);
            }
            catch(Exception ex)
            {
                chatTextBox.Text += "\n" + ex.Message;
            }
            messageBox.Text = "";
        }
    }
}
