
using Client.App.BusinessManager.Interface;
using Client.App.Infrastructure;
using Client.App.Model;
using Client.App.Services.Interface;

namespace Client.App.BusinessManager
{
    public class SimpleTransaction : ISimpleTransaction
    {
        private readonly IGenericMemoryCache _genericMemoryCache;
        private readonly IAuthentication _authentication;
        private readonly ITransaction _transaction;
        public SimpleTransaction(IGenericMemoryCache genericMemoryCache, IAuthentication authentication, ITransaction transaction)
        {
            _genericMemoryCache = genericMemoryCache;
            _authentication = authentication;
            _transaction = transaction;
        }
        public async Task RunAsync()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Simple Transaction Processing");

            var login = ReadLoginDetails();
            var accessToken = await _authentication.Login(login.UserName,login.Password);
            _genericMemoryCache.SetItem("BearerToken", accessToken.AccessToken);
           // client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.auth_token);
            Console.WriteLine("\n Login Successfull.");
            try
            {
                DisplayMenu();
                string key;
                while ((key = Console.ReadKey().KeyChar.ToString()) != "4")
                {
                    int.TryParse(key, out int keyValue);

                    switch (keyValue)
                    {
                        case 1:
                            await ShowBalance();
                            break;
                        case 2:
                            await MakeTransaction(TransactionType.Deposit);
                            break;
                        case 3:
                            await MakeTransaction(TransactionType.Withdrawal);
                            break;
                    }

                    Console.Write("Enter the option (number): ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("App interrupted.");
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("App closed.");
            }

            Console.ReadLine();
        }

        #region Private Member

        static Login ReadLoginDetails()
        {
            Console.WriteLine();
            Console.Write("Enter the user name: ");
            var username = Console.ReadLine();
            Console.Write("Enter the password: ");
            var password = Console.ReadLine();
            return new Login() { UserName = username, Password = password };
        }

        void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Balance");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Close app (X)");
            Console.WriteLine();
            Console.Write("Enter the option (number): ");
          
        }

        async Task ShowBalance()
        {
            Console.WriteLine();
            var transactionResult = await _transaction.BalanceAsync();

            Console.WriteLine();
            Console.WriteLine("Balance");
            Console.WriteLine();

            if (transactionResult.Balance != null)
            {
                Console.WriteLine($"Account No: {transactionResult.AccountNumber}");
                Console.WriteLine($"Balance: {transactionResult.Balance}");
                Console.WriteLine($"Currency: {transactionResult.Currency}");
            }
            else
            {
                Console.WriteLine($"Status: Transaction failed");
                Console.WriteLine($"Message: {transactionResult.Message}");
            }

            Console.WriteLine();
        }

        async Task MakeTransaction(TransactionType transactionType)
        {
            Console.WriteLine();

            Console.Write("Enter the Amount: ");
            var transactionAmount = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine(transactionType.ToString());
            Console.WriteLine();

            var transactionInput = new TransactionInput()
            {
                TransactionType = transactionType,
                Amount = Math.Round(Convert.ToDecimal(transactionAmount), 2)
            };

            var transactionResult = new TransactionResult();
            if (transactionType == TransactionType.Deposit)
            {
                transactionResult = await _transaction.DepositAsync(transactionInput);
            }
            else if (transactionType == TransactionType.Withdrawal)
            {
                transactionResult = await _transaction.WithdrawAsync(transactionInput);
            }

            if (transactionResult.IsSuccessful)
            {
                Console.WriteLine($"Status: {transactionResult.Message}");
                Console.WriteLine($"Account No: {transactionResult.AccountNumber}");
                Console.WriteLine($"Current Balance: {transactionResult.Balance}");
                Console.WriteLine($"Currency: {transactionResult.Currency}");
            }
            else
            {
                Console.WriteLine($"Status: Transaction failed");
                Console.WriteLine($"Message: {transactionResult.Message}");
            }

            Console.WriteLine();
        }

        #endregion
    }
}
