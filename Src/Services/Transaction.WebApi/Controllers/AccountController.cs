
using EventBus.Abstractions;
using Transaction.Framework.Domain;
using Transaction.Framework.Services.Interface;
using Transaction.Framework.Types;
using Transaction.WebApi.Models;


namespace Transaction.WebApi.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;

        public AccountController(ITransactionService transactionService, IIdentityService identityService,
            IEventBus eventBus)
        {
            _transactionService = transactionService;
            _identityService = identityService;
            _eventBus = eventBus;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> Balance()
        {
            var accountNumber = _identityService.GetIdentity().AccountNumber;
            var transactionResult = await _transactionService.Balance(accountNumber);

         
            _eventBus.Publish(new BalanceCheckEvent(transactionResult.AccountNumber, 
                transactionResult.IsSuccessful, transactionResult.Balance.Amount,
                transactionResult.Balance.Currency.ToString(), transactionResult.Message));
          
            return Ok(transactionResult.MapToTransactionResultModel());
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionModel accountTransactionModel)
        {
            var accountTransaction = new AccountTransaction()
            {
                AccountNumber = _identityService.GetIdentity().AccountNumber,
                Amount = new Money(accountTransactionModel.Amount, Currency.INR ),
                TransactionType = TransactionType.Deposit
            };
           
            accountTransaction.TransactionType = TransactionType.Deposit;
            var transactionResult = await _transactionService.Deposit(accountTransaction);
            
            _eventBus.Publish(new DepositEvent(transactionResult.AccountNumber,
               transactionResult.IsSuccessful, transactionResult.Balance.Amount,
               transactionResult.Balance.Currency.ToString(), transactionResult.Message));

            return Created(string.Empty, transactionResult.MapToTransactionResultModel());
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionModel accountTransactionModel)
        {
            var accountTransaction = new AccountTransaction()
            {
                AccountNumber = _identityService.GetIdentity().AccountNumber,
                Amount = new Money(accountTransactionModel.Amount, Currency.INR),
                TransactionType = TransactionType.Deposit
            };

        
            accountTransaction.TransactionType = TransactionType.Withdrawal;
            var transactionResult = await _transactionService.Withdraw(accountTransaction);

            _eventBus.Publish(new DepositEvent(transactionResult.AccountNumber,
               transactionResult.IsSuccessful, transactionResult.Balance.Amount,
               transactionResult.Balance.Currency.ToString(), transactionResult.Message));

            return Created(string.Empty, transactionResult.MapToTransactionResultModel());
        }
    }
}
