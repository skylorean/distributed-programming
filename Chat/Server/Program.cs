﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{
    public static void StartListening(int port)
    {
        // Разрешение сетевых имён

        // Привязываем сокет ко всем интерфейсам на текущей машинe
        IPAddress ipAddress = IPAddress.Any;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        // CREATE
        Socket listener = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        List<string> messages = [];

        try
        {
            // BIND
            listener.Bind(localEndPoint);

            // LISTEN
            listener.Listen(10);

            while (true)
            {
                Console.WriteLine("Ожидание соединения клиента...");
                // ACCEPT
                Socket handler = listener.Accept();

                Console.WriteLine("Получение данных...");
                byte[] buf = new byte[1024];
                string data = null;
                while (true)
                {
                    // RECEIVE
                    int bytesRec = handler.Receive(buf);

                    data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

                data = data.Substring(0, data.Length - "<EOF>".Length);
                Console.WriteLine("Полученный текст: {0}", data);

                messages.Add(data);

                // Отправляем текст обратно клиенту
                string textToSend = null;
                foreach (var i in messages)
                {
                    textToSend += i + "\n";

                }
                textToSend += "<EOF>";
                byte[] msg = Encoding.UTF8.GetBytes(textToSend);

                // SEND
                handler.Send(msg);

                // RELEASE
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
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
            Console.WriteLine("Запуск сервера...");
            StartListening(int.Parse(args[0]));
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }

        Console.WriteLine("\nНажмите ENTER чтобы выйти...");
        Console.Read();
    }
}