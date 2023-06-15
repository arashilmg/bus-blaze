namespace BizCover.Application.Renewals.Helpers
{
    public interface IQueuePublisher
    {
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken);
        Task Publish<TCommand>(TCommand command, CancellationToken cancellationToken);
    }
}
