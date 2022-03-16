

using System.Text.Json.Serialization;

namespace Notification.WebApi.IntegrationEvents.Events
{
    public record BalanceCheckEvent : IntegrationEvent
    {
        public int? AccountNumber { get; set; }
        public bool IsSuccessful { get; set; }
        public decimal? Balance { get; set; }
        public string Currency { get; set; }
        public string Message { get; set; }

    }
}
