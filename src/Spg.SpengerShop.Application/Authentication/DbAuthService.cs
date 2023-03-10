using Spg.SpengerShop.Domain.Exceptions;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Application.Authentication
{
    public class DbAuthService : IAuthService
    {
        private readonly IReadOnlyRepositoryBase<Customer> _customerRepository;

        public DbAuthService(IReadOnlyRepositoryBase<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<AuthInfos> CheckIdentity(string userName, string password)
        {
            // TODO: Datenbank verwenden
            if (userName != "martin" 
                || password != "vYXGf9Uc+BTs1v+otiqWxrFCNBDsRehYYvvMmUsz/j4=") // PWD: geheim
            {
                throw new AuthenticationException("Wrong Username or Password!");
            }
            AuthInfos authInfos = new AuthInfos(userName, "User", "Schrutek", "Martin");
            return authInfos;
        }
    }
}
