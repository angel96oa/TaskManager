namespace TaskManager.Messaging
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }

        // Constructor
        public RabbitMQConfiguration() { }
        public RabbitMQConfiguration(string hostName, string userName, string password, string queueName, string exchangeName)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
            QueueName = queueName;
            ExchangeName = exchangeName;
        }
    }
}
