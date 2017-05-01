using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace TempServer
{
    class Program
    {
        const int PORT = 8010;
        static TcpListener listener;
        static DBController DBcontr = new DBController("server", "port", "login", "password", "table");
        
        public delegate void Sender(string sent_from, string send_to, string message);

        static Dictionary<string, Sender> clients = new Dictionary<string, Sender>();
        public static bool AddClient(string username, string password, Sender send_func)
        {
            if (!DBcontr.CheckAuth(username, password))
            {
                send_func("SERVER", username, "EXIT");
                return false;
            }
            if (!clients.ContainsKey(username))
            {
                clients.Add(username, send_func);
                Console.WriteLine(username + " Connected");
            }
            else
            {
                RemoveClient(username);
                AddClient(username, password, send_func);
            }
            return true;
        }

        public static void AddRequest(string name_from, string name_to)
        {
            if (clients.ContainsKey(name_to))  // TODO: not in clients, but in base
            {
                clients[name_to]("SERVER", name_to, "ADD_"+name_from);
                clients[name_to](name_from, name_to, name_from + " добавил вас в друзья");
            }
            else
            {
                clients[name_from]("SERVER", name_from, name_to + " offline");
            }
        }

        public static void RemoveClient(string username)
        {
            if (clients.ContainsKey(username))
            {
                clients.Remove(username);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT);
                listener.Start();
                Console.WriteLine("Alive");
                Console.WriteLine("CTRL+C to exit");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clObject = new ClientObject(client);
                    clObject.Recived += MessageRecived;

                    Thread clientThread = new Thread(new ThreadStart(clObject.Process));
                    clientThread.Start();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void MessageRecived(string sent_from, string send_to, string message)
        {
            if (clients.ContainsKey(send_to))
            {
                clients[sent_from](sent_from, send_to, message);
                clients[send_to](sent_from, send_to, message);
            }
            else
            {
                clients[sent_from](sent_from, send_to, "Вне сети");
            }
        }

    }
}
