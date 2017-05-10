using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TempServer
{
    class ClientObject
    {
        const int NAME_LEN = 32;
        const string SERVER = "SERVER";

        public string username = null;

        public TcpClient client;
        NetworkStream stram = null;

        Queue<string> messages = new Queue<string>();

        public delegate void MessageReciver(string sent_from, string send_to, string message);
        public event MessageReciver Recived;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            Recive();
        }


        public void Recive()
        {
            try
            {
                stram = client.GetStream();
                byte[] data = new byte[64];

                while (true)
                {
                    //recive
                    StringBuilder strBuild = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stram.Read(data, 0, data.Length);
                        strBuild.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stram.DataAvailable);

                    string message = strBuild.ToString();
                    string send_to = message.Substring(0, NAME_LEN);
                    send_to = send_to.Substring(0, send_to.IndexOf('%'));
                    if (send_to == SERVER)
                    {
                        string command = message.Substring(NAME_LEN, 4);//TODO: 4 isn't beautiful
                        switch (command)
                        {
                            case "AUTH":
                                username = message.Substring(NAME_LEN + 4);
                                string password = username.Substring(username.LastIndexOf('%')+1);
                                username = username.Substring(0,username.IndexOf('%'));

                                if (!Program.AddClient(username, password, Send))
                                    throw new Exception(username + " Denied");
                                break;
                            case "ADD_":
                                string name_to = message.Substring(NAME_LEN+4);
                                Program.AddRequest(username, name_to);
                                break;
                            case "EXIT":
                                throw new Exception(username+" Disconnected");
                                break;
                            case "REG_":
                                string t_username = message.Substring(NAME_LEN + 4,NAME_LEN);
                                string t_password = message.Substring(NAME_LEN + 4 + NAME_LEN);
                                if(Program.RegistrationRequest(t_username, t_password))
                                {
                                    Send("SERVER", t_username, "REG_FINE");
                                    throw new Exception(username + " Registred");
                                }
                                else
                                {
                                    Send("SERVER", t_username, "REG_BAD");
                                    throw new Exception(username + " not registred");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (username != null)
                    {
                        string sent_from = username;
                        send_to = send_to;
                        message = message.Substring(NAME_LEN);
                        Recived.Invoke(sent_from, send_to, message);
                    }

                }
            }
            catch (Exception ex)
            {
                stram.Close();
                client.Close();
                //Program.RemoveClient(username); //TODO: safety threads;
                Console.WriteLine(ex.Message);

                return;
            }
            finally
            { 
                if(username!=null)
                    Program.RemoveClient(username);

                if (stram != null)
                    stram.Close();
                if (client != null)
                    client.Close();

            }
        }

        public void Send(string sent_from, string send_to, string message)
        {
            if (username != null)
            {
                byte[] data = new byte[64];
                if(sent_from!="SERVER")
                    data = Encoding.Unicode.GetBytes(sent_from+"^:"+message);
                else
                    data = Encoding.Unicode.GetBytes(sent_from + message);
                if (data.Length < 64)
                {
                    byte[] ndata = new byte[64];
                    ndata.Initialize();
                    data.CopyTo(ndata, 0);
                    data = ndata;
                }
                stram.Write(data, 0, data.Length);
            }
        }

    }
}
