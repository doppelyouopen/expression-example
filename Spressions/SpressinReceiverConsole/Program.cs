using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json;
using Spressin.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpressinReceiverConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            serializerSettings.Converters.Add(new ExpressionJsonConverter(Assembly.GetAssembly(typeof(Program))));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9011);

            Console.WriteLine("connected...");

            Person person = new Person() { Name = "Mahmood" };

            string jsonData = JsonConvert.SerializeObject(person);
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);

            socket.Send(dataBytes);

            Console.WriteLine("sent...");

            byte[] buffer = new byte[1024 * 4];
            int readBytes = socket.Receive(buffer);
            MemoryStream memoryStream = new MemoryStream();

            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);

                if (socket.Available > 0)
                {
                    readBytes = socket.Receive(buffer);
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("read...");

            byte[] totalBytes = memoryStream.ToArray();

            memoryStream.Close();

            string readData = Encoding.Default.GetString(totalBytes);

            //Greeting response = JsonConvert.DeserializeObject<Greeting>(readData);

            var exp = JsonConvert.DeserializeObject<Expression<Func<int, int>>>(readData, serializerSettings).Compile();

            int value = 0;
            while (!int.TryParse(Console.ReadLine(), out value)) {}

            Console.WriteLine($"f({value})={exp(value)}");

            Console.ReadKey();
        }
    }
}
