using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Lab6_ClientServer
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        public Client(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }

        public void SendMessage(Message message)
        {
            string json = JsonSerializer.Serialize(message);

            writer.WriteLine(json);
            writer.Flush();
        }

        public Message RecieveMessage()
        {
            string json = reader.ReadLine();

            Message message = JsonSerializer.Deserialize<Message>(json);

            return message;
        }

        public void Close()
        {
            client.Close();
            stream.Close();
            writer.Close();
            reader.Close();
        }

        public static void Main(string[] args)
        {

            Console.WriteLine("Client started");
            Client client = new Client("localhost", 12345);

            try
            {

                for (int i = 0; i < 10; i++)
                {
                    Message message = new Message(i);

                    client.SendMessage(message);

                    var messageRecv = client.RecieveMessage();

                    messageRecv.FindDifference(message);

                    Thread.Sleep(1000);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Server disconnected");
            }
            finally
            {
                Console.WriteLine("Client finish");
                client.Close();
            }
        }
    }
}