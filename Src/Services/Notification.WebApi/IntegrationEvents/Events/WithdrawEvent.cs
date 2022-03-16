
namespace Notification.WebApi.IntegrationEvents.Events
{
    public record WithdrawEvent : IntegrationEvent
    {

        public int? AccountNumber { get;  }
        public bool IsSuccessful { get;  }
        public decimal? Balance { get;  }
        public string Currency { get;  }
        public string Message { get;  }

     
    }
}
