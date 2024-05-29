using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab6_ClientServer
{
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;    
        private StreamReader reader;

        private bool isConnectionActive = true;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

        }

        public void Run()
        {
            try
            {
                while (isConnectionActive)
                {
                    string json = reader.ReadLine(); // blocking
                    Message message = JsonSerializer.Deserialize<Message>(json);

                    Console.WriteLine($"Received message with id {message.id} from {client.Client.RemoteEndPoint?.ToString()}, Modifing...");

                    Thread.Sleep(1000);

                    Message modifiedMessage = new Message(message.id + 1);


                    string jsonMessage = JsonSerializer.Serialize(modifiedMessage);
                    writer.WriteLine(jsonMessage);
                    writer.Flush();
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Client disconnected");
            } 
            finally
            {
                client.Close();
            }
        }

        public void Stop()
        {
            isConnectionActive = false;
            client.Close();
            stream.Close();
            writer.Close();
            reader.Close();
        }
    }
}
