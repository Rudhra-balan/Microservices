using Transaction.Framework.Domain;

namespace Transaction.WebApi.Models
{
    public class TransactionResultModel 
    {
        public int? AccountNumber { get; set; }
        public bool IsSuccessful { get; set; }
        public decimal? Balance { get; set; }
        public string Currency { get; set; }
        public string Message { get; set; }
    }

    public static class TransactionResultMapper
    {
        public static TransactionResultModel MapToTransactionResultModel(this TransactionResult transactionResult)
        {
            return new TransactionResultModel
            {
                AccountNumber = transactionResult.AccountNumber,
                Balance = transactionResult.Balance.Amount,
                IsSuccessful = transactionResult.IsSuccessful,
                Currency = transactionResult.Balance.Currency.ToString(),
                Message = transactionResult.Message

            };
        }
    }
}
