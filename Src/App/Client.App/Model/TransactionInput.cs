
using Client.App.Infrastructure;

namespace Client.App.Model
{
    public class TransactionInput
    {
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
    }

}
