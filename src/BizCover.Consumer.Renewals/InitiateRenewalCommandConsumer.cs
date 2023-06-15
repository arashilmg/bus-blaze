using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Renewals;
using MassTransit;

namespace BizCover.Consumer.Renewals
{
    public class InitiateRenewalCommandConsumer : IConsumer<InitiateRenewalCommand>
    {
        private readonly InitiateRenewal _initiateRenewal;

        public InitiateRenewalCommandConsumer(InitiateRenewal initiateRenewal)
        {
            _initiateRenewal = initiateRenewal;
        }

        public async Task Consume(ConsumeContext<InitiateRenewalCommand> context) =>
            await _initiateRenewal.Initiate(context.Message.ExpiringPolicyId, context.CancellationToken);
    }
}
