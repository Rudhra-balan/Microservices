





namespace Notification.WebApi.IntegrationEvents.EventHandling
{
    public class BalanceCheckEventHandler:IIntegrationEventHandler<BalanceCheckEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<BalanceCheckEventHandler> _logger;

        public BalanceCheckEventHandler(
            IHubContext<NotificationHub> hubContext,
            ILogger<BalanceCheckEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(BalanceCheckEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                await _hubContext.Clients
                    .Group(@event.AccountNumber.ToString())
                    .SendAsync("AccountBalance", new { AccountNumber = @event.AccountNumber,
                                                            Balance = @event.Balance,
                                                            Currency = @event.Currency });
            }
        }
    }
}
