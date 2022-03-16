namespace Transaction.WebApi.Services
{
    using Common.Lib.JwtTokenHandler;
    using Microsoft.AspNetCore.Http;
    using System;
   
    using System.Linq;
    using System.Security.Claims;
    using Transaction.WebApi.Models;

    public class IdentityService : IIdentityService
    {

        private readonly ClaimsPrincipal _user;
        public IdentityService(IHttpContextAccessor context)
        {
            _user = context.HttpContext?.User;
        }

        public IdentityModel GetIdentity()
        {

            if (_user != null)
            {
               
                var account = _user.Claims
                    .Where(c => c.Type == ClaimType.Accountnumber)
                    .FirstOrDefault();

                var name = _user.Claims
                    .Where(c => c.Type == ClaimType.GivenName)
                    .FirstOrDefault();

                var currency = _user.Claims
                    .Where(c => c.Type == ClaimType.Currency)
                    .FirstOrDefault();

                return new IdentityModel()
                {
                    AccountNumber = Convert.ToInt32(account.Value),
                    FullName = name.Value,
                    Currency = currency.Value
                };
            }

            throw new ArgumentNullException("accountnumber");
        }
    }
}
