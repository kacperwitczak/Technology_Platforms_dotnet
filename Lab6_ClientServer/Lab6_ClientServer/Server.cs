using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_ClientServer
{
    public class Server
    {
        private TcpListener listener;
        private bool isRunning = true;
        private List<ClientHandler> clients;

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            clients = new List<ClientHandler>();
        }

        public void Run()
        {
            listener.Start();
            Console.WriteLine("Server started");

            while (isRunning)
            {
                TcpClient connection = listener.AcceptTcpClient();

                if (!isRunning)
                {
                    break;
                }

                Console.WriteLine("Client connected");

                var client = new ClientHandler(connection);

                Thread thread = new Thread(client.Run);
                thread.Start();


                clients.Add(client);
            }
        }

        public async Task Stop()
        {
            isRunning = false;
            listener.Stop();

            var tasks = new List<Task>();
            foreach (var client in clients)
            {
                tasks.Add(Task.Run(() => client.Stop()));
            }

            await Task.WhenAll(tasks);
        }

        public static void Main(string[] args)
        {
            Server server = new Server(12345);
            var serverTask = Task.Run(() => server.Run());

            Console.ReadLine();

            server.Stop().Wait();
        }

    }
}
