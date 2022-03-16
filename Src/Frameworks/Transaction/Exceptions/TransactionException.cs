namespace Transaction.Framework.Exceptions
{
    using System;
    using Transaction.Framework.Types;

    public abstract class TransactionException : Exception
    {
        public TransactionException(string message)
            : base(message)
        {  }

        public abstract string Type { get; }
        public abstract int ErrorCode { get; }
    }

    public class InsufficientBalanceException : TransactionException
    {
        public InsufficientBalanceException()
        : base($"This operation can not be performed due to insufficient balance in the account.")
        { }

        public override int ErrorCode => 
            TransactionErrorCode.InsufficientBalance;

        public override string Type =>
            TransactionErrorCode.InsufficientBalance.ToString();
    }

    public class InvalidAccountNumberException : TransactionException
    {
        public InvalidAccountNumberException(int accountNumber)
        : base($"This account {accountNumber} does not exist.")
        { }

        public override int ErrorCode => 
            TransactionErrorCode.AccountDoesNotExistError;

        public override string Type =>
          TransactionErrorCode.AccountDoesNotExistError.ToString();
    }

    public class InvalidAmountException : TransactionException
    {
        public InvalidAmountException(decimal amount)
        : base($"This operation can not be performed for {amount} amount.")
        { }

        public override int ErrorCode => 
            TransactionErrorCode.InvalidAmount;

        public override string Type =>
        TransactionErrorCode.InvalidAmount.ToString();
    }

    public class InvalidCurrencyException : TransactionException
    {
        public InvalidCurrencyException(string currency)
            : base($"This operation can not be performed with {currency} currency.")
        { }

        public override int ErrorCode => 
            TransactionErrorCode.InvalidCurrencyError;
        public override string Type =>
             TransactionErrorCode.InvalidCurrencyError.ToString();
    }

    public class CurrencyMismatchException : TransactionException
    {
        public CurrencyMismatchException(Currency c1, Currency c2)
            : base($"This operation cannot be performed between {c1} and {c2}.")
        { }

        public override int ErrorCode => 
            TransactionErrorCode.CurrencyMismatchError;

        public override string Type =>
            TransactionErrorCode.CurrencyMismatchError.ToString();
    }


}
