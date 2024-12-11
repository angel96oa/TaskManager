using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace TaskManager.Messaging
{
    public class RabbitMQService : IDisposable
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQConfiguration _config;
        private bool _disposed;

        public RabbitMQService(ILogger<RabbitMQService>  logger, IOptions<RabbitMQConfiguration> config)
        {
            _logger = logger;
            _config = config.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _config.ExchangeName,
                                     type: ExchangeType.Direct, 
                                     durable: true,              
                                     autoDelete: false);         

            _channel.QueueDeclare(queue: _config.QueueName,
                                  durable: true,          
                                  exclusive: false,       
                                  autoDelete: false,      
                                  arguments: null);       

            _channel.QueueBind(queue: _config.QueueName,
                               exchange: _config.ExchangeName,
                               routingKey: _config.QueueName);  
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            try
            {
                _channel.BasicPublish(
                    exchange: _config.ExchangeName,
                    routingKey: _config.QueueName,
                    basicProperties: null,
                    body: body);
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Exception sending message to queue");
            }

        }

        public void ReceiveMessages()
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"Received Message: {message}");
                };

                _channel.BasicConsume(queue: _config.QueueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Exception consuming message from queue");
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _channel?.Close();
                    _connection?.Close();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
