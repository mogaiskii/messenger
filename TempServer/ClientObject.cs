using System;
using System.Net.Sockets;
using System.Text;

namespace TempServer
{
    class ClientObject
    {
        public TcpClient client;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stram = null;
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

                    //TODO: debug
                    string message = strBuild.ToString();
                    Console.WriteLine(message);
                    message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                    data = Encoding.Unicode.GetBytes(message);
                    stram.Write(data,0,data.Length);
                }
            }
            catch(Exception ex)
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

    }
}
