

using Client.App.Model;

namespace Client.App.Services.Interface
{
    public interface ITransaction
    {
        Task<TransactionResult> BalanceAsync();

        Task<TransactionResult> DepositAsync(TransactionInput transactionDetails);

        Task<TransactionResult> WithdrawAsync(TransactionInput transactionDetails);
    }
}
