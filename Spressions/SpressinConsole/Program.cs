using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.IO;
using Newtonsoft.Json;
using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Net.Sockets;
using System.Net;
using Spressin.Core;

namespace SpressinConsole
{
    class Program
    {
        //public class JsonNetAdapter
        //{
        //    private readonly JsonSerializerSettings _settings;

        //    public JsonNetAdapter(JsonSerializerSettings settings = null)
        //    {
        //        var defaultSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        //        defaultSettings.Converters.Add(new ExpressionJsonConverter(Assembly.GetAssembly(typeof(IOconSituation))));
        //        _settings = settings ?? defaultSettings;
        //    }

        //    public string Serialize<T>(T obj)
        //    {
        //        return JsonConvert.SerializeObject(obj, _settings);
        //    }

        //    public T Deserialize<T>(string json)
        //    {
        //        return JsonConvert.DeserializeObject<T>(json, _settings);
        //    }
        //}


        static void Main(string[] args)
        {
            CamelCasePropertyNamesContractResolver c;
            //ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "int32"); // int32 parameter.
            //ConstantExpression constantExpression = Expression.Constant(0, typeof(int)); // 0
            //BinaryExpression greaterThanExpression = Expression.GreaterThan(
            //    left: parameterExpression, right: constantExpression); // int32 > 0

            //Expression<Func<int, bool>> isPositiveExpression = Expression.Lambda<Func<int, bool>>(
            //    body: greaterThanExpression, // ... => int32 > 0
            //    parameters: parameterExpression); // int32 => ...

            //Console.WriteLine(isPositiveExpression.Compile().Invoke(-3));

            //ParameterExpression bP = Expression.Parameter(typeof(bool));
            //BinaryExpression und = Expression.And(bP, bP);
            //Console.WriteLine(Expression.Lambda<Func<bool, bool, bool>>(und, bP, bP).Compile().Invoke(true, false));

            var serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            serializerSettings.Converters.Add(new ExpressionJsonConverter(Assembly.GetAssembly(typeof(Program))));


            //Expression <Func<int, int>> a = x => x + 1;

            //File.WriteAllText(@"C:\EasyFind\expression.json", JsonConvert.SerializeObject( a, serializerSettings));

            //Expression<Func<int, int>> a = JsonConvert.DeserializeObject<Expression<Func<int, int>>>(File.ReadAllText(@"C:\EasyFind\expression.json"), serializerSettings);

            //Console.WriteLine(a.Compile()(3));
            //Console.ReadLine();


            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 9011);

            socket.Bind(endPoint);
            socket.Listen(5);

            while (true)
            {
                Console.WriteLine("waiting for new connection...");

                Socket newSocket = socket.Accept();

                MemoryStream memoryStream = new MemoryStream();

                Console.WriteLine("new connection...");

                byte[] buffer = new byte[1024];

                int readBytes = newSocket.Receive(buffer);

                while (readBytes > 0)
                {
                    memoryStream.Write(buffer, 0, readBytes);

                    if (socket.Available > 0)
                    {
                        readBytes = newSocket.Receive(buffer);
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("data received...");

                byte[] totalBytes = memoryStream.ToArray();

                memoryStream.Close();

                string readData = Encoding.Default.GetString(totalBytes);

                Expression<Func<int, int>> fn = x => (int)Math.Sqrt(x)*2;

                string dataToSend = JsonConvert.SerializeObject(fn, serializerSettings);

                byte[] dataToSendBytes = Encoding.Default.GetBytes(dataToSend);

                newSocket.Send(dataToSendBytes);

                newSocket.Close();

                Console.WriteLine("data sent...");
            }
        }
        static Greeting SayHello(Person g)
        {
            return new Greeting { Msg = $"Hello {g.Name}" };
        }
    }
}
