using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string exchangeName = "TestChange";
            string queueName = "hello";
            string routeKey = "helloRouteKey";

            //创建连接工厂
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",//用户名
                Password = "guest",//密码
                HostName = "localhost"//rabbitmq ip
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //定义一个Direct类型交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null);

            //定义一个队列
            channel.QueueDeclare(queueName, false, false, false, null);

            //将队列绑定到交换机
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            Console.WriteLine($"\nRabbitMQ Connect Success,Exchange：{exchangeName}，Queue：{queueName}，Route：{routeKey}，\n\n请输入消息，输入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }
    }
}