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
        //const string address = "127.0.0.1"; // TODO: debug

        const string address = "80.253.235.36";

        TcpClient client = null;
        NetworkStream stream = null;
        bool listen = false;
        delegate void ChatWriteDelegate(string message);
        delegate void ChangeGuiDelegate();
        delegate string GetSelectedFriend();

        string username = "";
        string token = null;
        Dictionary<string, Queue<string>> friends = new Dictionary<string, Queue<string>>();

        public Form1()
        {
            InitializeComponent();
           
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            notice.ShowBalloonTip(2000, "Messenger", "Hello! I'm here!", ToolTipIcon.Info);
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
            if (nameTextBox.InvokeRequired) //TODO: make a function from this
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
                    friends.Clear();
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
                friends.Clear();
                friendsList.Items.Clear();
                friendAddButton.Enabled = false;
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (username.Length == 0)
            {
                if (nameTextBox.Text.Length <= NAME_LEN && nameTextBox.Text.Length > 0
                    && !nameTextBox.Text.Contains("%") && nameTextBox.Text[0] != ' '
                    && nameTextBox.Text != "SERVER")
                {
                    Register();
                }
                else
                {
                    chatTextBox.Text += "\n!-- Name is incorrect !\nRegistration failed";
                }
            }
        }

        private void Register()
        {
            try
            {
                client = new TcpClient(address, PORT);
                stream = client.GetStream();

                username = nameTextBox.Text;
                string password = passwordTextBox.Text.GetHashCode().ToString();

                listen = true;
                Task reciveTask = new Task(Recive);
                reciveTask.Start();
                chatTextBox.Text = "";

                Send("SERVER", "REG_" + username +
                    string.Concat(Enumerable.Range(0, (NAME_LEN - username.Length)).Select(i => "%")) +
                    password);
            }
            catch (Exception ex)
            {
                chatTextBox.Text += '\n' + ex.Message;
                Logout();
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
                        string sender_username = message.Substring(0, message.IndexOf("^"));
                        string reciver_username = message.Substring(message.IndexOf("^") + 1,
                            message.IndexOf(":") - message.IndexOf("^") - 1);

                        message = "\n" + sender_username + "^:" + message.Substring(message.IndexOf(":"));
                        
                        if (sender_username != username)
                        {
                            //TODO: actually, it shouldn't happen.. let me thing about it..
                            ChangeGuiDelegate gui_del = delegate ()
                            {
                                if (!friends.ContainsKey(sender_username))
                                {
                                    friends.Add(sender_username, new Queue<string>());
                                }
                                friends[sender_username].Enqueue(message);
                            };
                            friendsList.Invoke(gui_del);

                            GetSelectedFriend friendDel = delegate ()
                            {
                                if(friendsList.SelectedItem == null)
                                {
                                    return "";
                                }
                                return friendsList.SelectedItem.ToString();
                            };

                            string selectedFriend = friendsList.Invoke(friendDel).ToString();
                            
                            if(selectedFriend == sender_username)
                            {
                                ChatWriteDelegate del = delegate (string mes) { chatTextBox.Text += message; };
                                chatTextBox.Invoke(del, message);
                            }
                            else
                            {
                                message = message.Substring(message.IndexOf(":"));
                                if(message.Length > 20)
                                {
                                    message = message.Substring(0, 18) + "..";
                                }
                                ChangeGuiDelegate notice_del = delegate ()
                                { 
                                    notice.ShowBalloonTip(4000, sender_username, message, ToolTipIcon.Info);
                                };
                                chatTextBox.Invoke(notice_del);
                            }
                        }
                        else
                        {
                            ChangeGuiDelegate message_del = delegate ()
                            {
                                if (friends.ContainsKey(reciver_username))
                                {
                                    friends[reciver_username].Enqueue(message);
                                }
                                if (friendsList.SelectedItem.ToString() == reciver_username)
                                {
                                    chatTextBox.Text += message;
                                }
                            };
                            chatTextBox.Invoke(message_del);
                        }
                        
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
                                if (friend.IndexOf('\0') != -1)
                                {
                                    friend = friend.Substring(0, friend.IndexOf('\0'));
                                }
                                ChangeGuiDelegate friend_del = delegate ()
                                {
                                    addFriend(friend);
                                };
                                friendsList.Invoke(friend_del);
                                break;
                            case "NADD":
                                string off_user = message.Substring(10, message.Length - 10);
                                ChatWriteDelegate del = delegate (string user) { chatTextBox.Text += user+" offline"; };
                                chatTextBox.Invoke(del, off_user);
                                break;
                            case "REG_":
                                if (message.Substring(10).Contains("FINE"))
                                {
                                    throw new Exception("\n\nRegistraion successful!\n\n");
                                    //ChatWriteDelegate rega = delegate (string text) { chatTextBox.Text = "\n\nRegistraion successful!\n\n"; };
                                    //chatTextBox.Invoke(rega,"");
                                }
                                else
                                {
                                    throw new Exception("Registraion failed!\n");
                                }
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

        private void sendButton_Click(object sender, EventArgs e)
        {
            string message = messageBox.Text;

            string send_to = friendsList.SelectedItem.ToString();

            message = message.Substring(message.IndexOf('%', 1) + 1);

            try
            {
                Send(send_to, message);
            }
            catch (Exception ex)
            {
                chatTextBox.Text += "\n" + ex.Message;
            }
            messageBox.Text = "";
            chatTextBox.ScrollToCaret();
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

        private void friendAddButton_Click(object sender, EventArgs e)
        {
            string name = friendTextBox.Text;
            if(name.Length > 0 && name.Length <= 32 && name != "SERVER" && name != username) // TODO: &&name!=username
            {
                addFriend(name);
                Send("SERVER", "ADD_" + name);
            }
            friendTextBox.Text = "";
        }

        private void addFriend(string name)
        {
            friendsList.Items.Add(name);
            friends.Add(name, new Queue<string>());
        }

        private void friendsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            chatTextBox.Clear();
            string friend_name = friendsList.SelectedItem.ToString();
            Queue<string> messagesQueue = friends[friend_name];
            foreach (var message in messagesQueue)
            {
                chatTextBox.Text += message;
            }
            chatTextBox.ScrollToCaret();
        }
        
        private void Form1_Closing(object sender, EventArgs e)
        {
            Logout();
        }

    }
}
