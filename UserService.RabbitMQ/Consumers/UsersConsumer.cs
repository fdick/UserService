using MassTransit;

namespace UserService.RabbitMQ.Consumers
{
    public class UsersConsumer : IConsumer<UserMQEvent>
    {
        public Task Consume(ConsumeContext<UserMQEvent> context)
        {
            var b = context.ReceiveContext.GetBody();
            return Task.CompletedTask;
        }
    }

    public record UserMQEvent(Guid id, string name, string lastname, string nickname, string email);
}
