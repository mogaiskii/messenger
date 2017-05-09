using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

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
        string token = null;
        Dictionary<string, Queue<string>> friends = new Dictionary<string, Queue<string>>();

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
            passwordTextBox.ReadOnly = true;
            passwordTextBox.Enabled = false;
            sendButton.Enabled = true;
            registerButton.Enabled = false;
            loginButton.Text = "Logout";
            chatTextBox.BackColor = System.Drawing.Color.LightGreen;
            //TODO: try to load friends
            friendAddButton.Enabled = true;

            try
            {
                client = new TcpClient(address, PORT);
                stream = client.GetStream();

                string password = passwordTextBox.Text.GetHashCode().ToString();

                listen = true;
                Task reciveTask = new Task(Recive);
                reciveTask.Start();
                chatTextBox.Text = "";

                Send("SERVER", "AUTH" + username+
                    string.Concat(Enumerable.Range(0, (NAME_LEN - username.Length)).Select(i => "%"))+
                    password);
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
                    passwordTextBox.ReadOnly = false;
                    passwordTextBox.Enabled = true;
                    sendButton.Enabled = false;
                    registerButton.Enabled = true;
                    loginButton.Text = "Login";
                    chatTextBox.BackColor = System.Drawing.Color.LightGreen;
                    friendsList.Items.Clear();
                    friendAddButton.Enabled = false;
                };
                nameTextBox.Invoke(del);
            }
            else
            {
                nameTextBox.ReadOnly = false;
                nameTextBox.Enabled = true;
                passwordTextBox.ReadOnly = false;
                passwordTextBox.Enabled = true;
                sendButton.Enabled = false;
                registerButton.Enabled = true;
                loginButton.Text = "Login";
                chatTextBox.BackColor = System.Drawing.Color.LightGreen;
                friendsList.Items.Clear();
                friendAddButton.Enabled = false;
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
                    string message = strBuild.ToString();
                    if (message.Substring(0, 6) != "SERVER")
                    {
                        message = "\n" + message;
                        ChatWriteDelegate del = delegate (string mes) { chatTextBox.Text += message; };
                        chatTextBox.Invoke(del, message);
                        //chatTextBox.Text += message;
                    }
                    else
                    {
                        switch (message.Substring(6, 4))
                        {
                            case "AUTH":
                                token = message.Substring(10, message.Length - 10);
                                break;
                            case "EXIT":
                                throw new Exception("Invalid password");
                                break;
                            case "ADD_":
                                string friend = message.Substring(10);
                                ChangeGuiDelegate friend_del = delegate ()
                                {
                                    addFriend(friend);
                                    friendsList.ClearSelected();
                                    friendsList.SetSelected(friendsList.Items.Count - 1, true);
                                };
                                friendsList.Invoke(friend_del);
                                break;
                            case "NADD":
                                string off_user = message.Substring(10, message.Length - 10);
                                ChatWriteDelegate del = delegate (string user) { chatTextBox.Text += user+" offline"; };
                                chatTextBox.Invoke(del, off_user);
                                break;
                        }
                    }
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
            
            string send_to = friendsList.SelectedItem.ToString();

            message = message.Substring(message.IndexOf('%',1)+1);

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

        private void friendAddButton_Click(object sender, EventArgs e)
        {
            string name = friendTextBox.Text;
            if(name.Length > 0 && name.Length <= 32 && name != "SERVER" && name != username) // TODO: &&name!=username
            {
                addFriend(name);
                Send("SERVER", "ADD_" + name);
            }
        }

        private void addFriend(string name)
        {
            friendsList.Items.Add(name);
            friends.Add(username, new Queue<string>());
        }

        private void friendsList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
