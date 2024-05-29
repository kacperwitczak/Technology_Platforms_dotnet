using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_ClientServer
{
    [Serializable]
    public class Message
    {
        public int id { get; set; }

        public Message(int id)
        {
            this.id = id;
        }

        public void FindDifference(Message message)
        {
            if (this.id != message.id)
            {
                Console.WriteLine($"Difference between {this.id} and {message.id} is {this.id - message.id}");
            }
        }
    }
}
