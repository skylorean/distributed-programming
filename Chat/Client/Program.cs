using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    public static void StartClient(string host, int port, string message)
    {
        try
        {
            // Разрешение сетевых имён
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // CREATE
            Socket sender = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                // CONNECT
                sender.Connect(remoteEP);

                Console.WriteLine("Удалённый адрес подключения сокета: {0}",
                    sender.RemoteEndPoint.ToString());

                // Подготовка данных к отправке
                byte[] msg = Encoding.UTF8.GetBytes(message);

                // SEND
                int bytesSent = sender.Send(msg);


                // RECEIVE
                byte[] buf = new byte[1024];

                string data = null;
                while (true)
                {
                    // RECEIVE
                    int bytesRec = sender.Receive(buf);

                    data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    if (bytesRec <= 0)
                    {
                        break;
                    }
                }

                //Console.WriteLine(data);
                Console.WriteLine("Ответ: {0}",
                    data);

                // RELEASE
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void Main(string[] args)
    {
        try
        {
            StartClient("127.0.0.1", 7000, "hoho!<EOF>");
            //StartClient(args[0], int.Parse(args[1]), args[2]);

        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }
}