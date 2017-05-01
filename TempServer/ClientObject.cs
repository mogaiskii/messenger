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
                                Program.clients.Add(username, Send);
                                break;
                            case "EXIT":
                                username = null; //TODO: brea
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
                Program.clients.Remove(username);
                Console.WriteLine(ex.Message);

                return;
            }
            finally
            {
                if (stram != null)
                    stram.Close();
                if (client != null)
                    client.Close();

                Program.clients.Remove(username);
            }
        }

        public void Send(string sent_from, string send_to, string message)
        {
            if (username != null)
            {
                byte[] data = new byte[64];
                data = Encoding.Unicode.GetBytes(sent_from+"^:"+message);
                stram.Write(data, 0, data.Length);
            }
        }

    }
}
