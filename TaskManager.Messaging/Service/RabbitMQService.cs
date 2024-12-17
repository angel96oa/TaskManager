using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Messaging
{
    public class RabbitMQService : IDisposable
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQConfiguration _config;
        private bool _disposed;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _listeningTask;
        private string _filePath;

        public RabbitMQService(ILogger<RabbitMQService> logger,
                               IOptions<RabbitMQConfiguration> config)
        {
            _logger = logger;
            _config = config.Value;

            _filePath = "..\\logs\\";
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

        public void StartListening()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            // Start the task that listens to the queue continuously
            _listeningTask = Task.Run(() => ReceiveMessages(token), token);
        }

        public void StopListening()
        {
            _cancellationTokenSource?.Cancel();
            _listeningTask?.Wait();
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

        private void ReceiveMessages(CancellationToken cancellationToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Cancellation requested. Stopping message consumption.");
                        return;
                    }

                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation($"Received Message: {message}");

                    // Escribir el mensaje en el archivo utilizando FileWriteService
                    WriteMessageToFile(message);
                };

                _channel.BasicConsume(queue: _config.QueueName,
                                     autoAck: true,
                                     consumer: consumer);

                // Bloquear la tarea hasta que se cancele
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(100); // Prevent high CPU usage while waiting for messages
                }
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

        private void WriteMessageToFile(string message)
        {
            try
            {
                // Asegúrate de que el directorio donde se guarda el archivo exista
                string directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Escribir el mensaje en el archivo
                using (StreamWriter writer = new StreamWriter(_filePath+DateTime.UtcNow.Day + "__" + DateTime.UtcNow.Month + "_" + "_" + DateTime.UtcNow.Year+".txt", append: true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }

                _logger.LogInformation($"Message written to file: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing message to file");
            }
        }
    }
}
