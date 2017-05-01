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
        static object locker = new object();
        
        public delegate void Sender(string sent_from, string send_to, string message);

        static Dictionary<string, Sender> clients = new Dictionary<string, Sender>();
        public static void AddClient(string username, Sender send_func)
        {
            if (!clients.ContainsKey(username))
            {
                clients.Add(username, send_func);
                Console.WriteLine(username + " Connected");
            }
            else
            {
                RemoveClient(username);
                AddClient(username, send_func);
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
