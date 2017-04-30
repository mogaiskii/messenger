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

        static Queue<string> messages = new Queue<string>();


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

        public static void MessageRecived(string message)
        {
            messages.Enqueue(message);
        }

    }
}
