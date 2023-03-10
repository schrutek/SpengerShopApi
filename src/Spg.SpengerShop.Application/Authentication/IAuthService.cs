using Spg.SpengerShop.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Application.Authentication
{
    public interface IAuthService
    {
        Task<AuthInfos> CheckIdentity(string userName, string password);
    }
}
