using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TaskManager.Messaging
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQConfiguration _config;
        private bool _disposed;

        public RabbitMQService(IOptions<RabbitMQConfiguration> config)
        {
            _config = config.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password
            };

            // Crear la conexión y el canal
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declarar el exchange (si no existe)
            _channel.ExchangeDeclare(exchange: _config.ExchangeName,
                                     type: ExchangeType.Direct, // Tipo de exchange (puede ser Direct, Topic, Fanout, etc.)
                                     durable: true,              // El exchange sobrevive a reinicios del servidor RabbitMQ
                                     autoDelete: false);         // No se elimina cuando no haya más consumidores

            // Declarar la cola (si no existe)
            _channel.QueueDeclare(queue: _config.QueueName,
                                  durable: true,          // La cola sobrevive a reinicios del servidor RabbitMQ
                                  exclusive: false,       // La cola no se borra cuando se cierra la conexión
                                  autoDelete: false,      // La cola no se elimina cuando no haya consumidores
                                  arguments: null);       // Opcional, puede ser usado para configurar argumentos adicionales

            // Enlazar la cola al exchange con la misma ruta (bindingKey)
            _channel.QueueBind(queue: _config.QueueName,
                               exchange: _config.ExchangeName,
                               routingKey: _config.QueueName);  // La key de enrutamiento será el nombre de la cola
        }

        // Método para enviar un mensaje a la cola
        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: _config.ExchangeName,  // Usamos el exchange de la configuración
                routingKey: _config.QueueName,   // Usamos la cola de la configuración
                basicProperties: null,
                body: body);
        }

        public void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Mensaje recibido: {message}");
                // Aquí procesas el mensaje
            };

            _channel.BasicConsume(queue: _config.QueueName,
                                 autoAck: true,
                                 consumer: consumer);
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
