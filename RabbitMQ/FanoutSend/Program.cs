using RabbitMQ.Client;
using System;
using System.Text;

namespace FanoutSend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string exchangeName = "TestFanoutChange";
            string queueName1 = "hello1";
            string queueName2 = "hello2";
            string routeKey = "";

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
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, false, null);//第三个参数表示交换机持久化

            //定义队列1
            channel.QueueDeclare(queueName1, true, false, false, null);//第二个参数表示队列持久化
            //定义队列2
            //channel.QueueDeclare(queueName2, false, false, false, null);

            //将队列绑定到交换机
            channel.QueueBind(queueName1, exchangeName, routeKey, null);
            //channel.QueueBind(queueName2, exchangeName, routeKey, null);
            IBasicProperties props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;//2代表队列中的消息持久化
            props.Expiration = "36000000";
            //生成两个队列的消费者
            //ConsumerGenerator(queueName1);
            //ConsumerGenerator(queueName2);

            Console.WriteLine($"\nRabbitMQ连接成功，\n\n请输入消息，输入exit退出！");

            //string input;
            //do
            //{
            //    input = Console.ReadLine();

            //    var sendBytes = Encoding.UTF8.GetBytes(input);
            //    //发布消息
            //    channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
            //} while (input.Trim().ToLower() != "exit");
            for (int i = 0; i < 1000; i++)
            {
                var sendBytes = Encoding.UTF8.GetBytes(i.ToString());
                //发布消息

                channel.BasicPublish(exchangeName, routeKey, props, sendBytes);
            }
            channel.Close();
            connection.Close();
        }
    }
}