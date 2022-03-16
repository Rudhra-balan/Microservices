

using Client.App.Infrastructure.Extensions;
using Client.App.Model;
using Client.App.Services.Interface;
using System.Net.Http.Json;

namespace Client.App.Services
{
    public class Transaction : ITransaction
    {
        private readonly HttpClient _client;
        private readonly IGenericMemoryCache _genericMemoryCache;
        public Transaction(HttpClient client, IGenericMemoryCache genericMemoryCache)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _genericMemoryCache = genericMemoryCache ?? throw new ArgumentNullException(nameof(genericMemoryCache));
        }

        public async Task<TransactionResult> BalanceAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _genericMemoryCache.GetItem("BearerToken").ToString());
            var response = await _client.GetAsync($"/account/balance");
            return await response.ReadContentAs<TransactionResult>();
        }

        public async Task<TransactionResult> DepositAsync(TransactionInput transactionDetails)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _genericMemoryCache.GetItem("BearerToken").ToString());
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");

            HttpResponseMessage response = await _client.PostAsJsonAsync("/account/deposit", transactionDetails);
           
            return await response.ReadContentAs<TransactionResult>();
        }

        public async Task<TransactionResult> WithdrawAsync(TransactionInput transactionDetails)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _genericMemoryCache.GetItem("BearerToken").ToString());
            HttpResponseMessage response = await _client.PostAsJson("/account/withdraw", transactionDetails);
            return await response.ReadContentAs<TransactionResult>();
        }
    }
}
