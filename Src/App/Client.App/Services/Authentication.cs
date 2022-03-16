

using Client.App.Infrastructure.Extensions;
using Client.App.Services.Interface;

namespace Client.App.Services
{
  
    public class Authentication : IAuthentication
    {
        private readonly HttpClient _client;

        public Authentication(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<TokenModel> Login(string userName,string password)
        {
            var response = await _client.PostAsJson($"/identity/Authenticate", new { username = userName, password = password});
            return await response.ReadContentAs<TokenModel>();
        }
    }
}
