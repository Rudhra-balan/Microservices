

namespace Client.App.Services.Interface
{
    public interface IAuthentication
    {
        Task<TokenModel> Login(string userName, string password);
    }
}
