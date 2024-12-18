namespace TaskManager.Messaging
{
    public interface IRabbitMQService
    {
        void StartListening();
        void StopListening();
        void SendMessage(string message);
        void ReceiveMessages(CancellationToken cancellationToken);
        void WriteMessageToFile(string message);
    }
}
