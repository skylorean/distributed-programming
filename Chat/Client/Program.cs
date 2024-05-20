﻿using System;
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
            IPAddress ipAddress;
            bool gotAddr = IPAddress.TryParse(host, out ipAddress);

            if (!gotAddr)
            {
                IPHostEntry dnsInfo = Dns.GetHostEntry(host);

                ipAddress = dnsInfo.AddressList[1];
            }

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

                // Подготовка данных к отправке
                byte[] msg = Encoding.UTF8.GetBytes(message + "<EOF>");

                // SEND
                int bytesSent = sender.Send(msg);

                // RECEIVE
                byte[] buf = new byte[1024];
                string data = null;
                while (true)
                {
                    int bytesRec = sender.Receive(buf);

                    data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

                data = data.Substring(0, data.Length - "<EOF>".Length);

                Console.Write(data);

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
            //StartClient("localhost", 7999, "hoho");
            StartClient(args[0], int.Parse(args[1]), args[2]);

        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }
}