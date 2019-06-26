using SolarLab.Common.Contracts;

namespace SevSharks.SignalR.Contracts
{
    /// <summary>
    /// Событие для SignalR
    /// </summary>
    public class SignalREvent : IWithQueueName
    {
        /// <summary>
        /// Имя очереди для события
        /// </summary>
        public string QueueName => Queues.SignalRQueue;

        /// <summary>
        /// Идентификатор пользователя - сообщение для пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public SignalRNotificationType Type { get; set; }
    }
}
