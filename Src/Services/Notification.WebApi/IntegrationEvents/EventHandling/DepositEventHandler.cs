





namespace Notification.WebApi.IntegrationEvents.EventHandling
{
    public class DepositEventHandler : IIntegrationEventHandler<DepositEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<BalanceCheckEventHandler> _logger;

        public DepositEventHandler(
            IHubContext<NotificationHub> hubContext,
            ILogger<BalanceCheckEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(DepositEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                await _hubContext.Clients
                    .Group(@event.AccountNumber.ToString())
                    .SendAsync("Deposit", new { AccountNumber = @event.AccountNumber,
                                                            Balance = @event.Balance,
                                                            Currency = @event.Currency });
            }
        }
    }
}
