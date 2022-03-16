using EventBus.Events;

namespace Notification.WebApi.IntegrationEvents.Events
{
    public record DepositEvent : IntegrationEvent
    {

        public int? AccountNumber { get;  }
        public bool IsSuccessful { get;  }
        public decimal? Balance { get;  }
        public string Currency { get;  }
        public string Message { get;  }
    }
}
