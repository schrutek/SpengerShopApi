using Spg.SpengerShop.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Application.Authentication
{
    public class AdAuthService : IAuthService
    {
        public async Task<AuthInfos> CheckIdentity(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
