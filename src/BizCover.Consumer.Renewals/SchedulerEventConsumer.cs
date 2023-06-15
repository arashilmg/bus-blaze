using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Scheduler;
using MassTransit;

namespace BizCover.Consumer.Renewals
{
    public class SchedulerEventConsumer : IConsumer<StatusChange>
    {
        private readonly StartAutoRenewalOrderGeneration _startAutoRenewalOrderGeneration;
        private readonly StartAutoRenewalOrderSubmission _startAutoRenewalOrderSubmission;
        private readonly StartRenewalInitiation _startRenewalInitiation;
        private readonly PublishDueForRenewalEvents _publishDueForRenewalEvents;

        public SchedulerEventConsumer(
            StartAutoRenewalOrderGeneration startAutoRenewalOrderGeneration,
            StartAutoRenewalOrderSubmission startAutoRenewalOrderSubmission,
            StartRenewalInitiation startRenewalInitiation,
            PublishDueForRenewalEvents publishDueForRenewalEvents)
        {
            _startAutoRenewalOrderGeneration = startAutoRenewalOrderGeneration;
            _startAutoRenewalOrderSubmission = startAutoRenewalOrderSubmission;
            _startRenewalInitiation = startRenewalInitiation;
            _publishDueForRenewalEvents = publishDueForRenewalEvents;
        }

        public async Task Consume(ConsumeContext<StatusChange> context)
        {
            await _startRenewalInitiation.Run(context.CancellationToken);
            await _startAutoRenewalOrderGeneration.Run(context.CancellationToken);
            await _startAutoRenewalOrderSubmission.Run(context.CancellationToken);
            await _publishDueForRenewalEvents.Run(context.CancellationToken);
        }
    }
}
