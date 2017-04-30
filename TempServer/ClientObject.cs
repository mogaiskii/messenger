using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TempServer
{
    class ClientObject
    {
        public TcpClient client;
        NetworkStream stram = null;

        Queue<string> messages = new Queue<string>();

        public delegate void MessageReciver(string message);
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
                    Recived.Invoke(message);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return;
            }
            finally
            {
                if (stram != null)
                    stram.Close();
                if (client != null)
                    client.Close();
            }
        }

        public void Send()
        {

        }

    }
}
