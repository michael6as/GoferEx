using System.Threading.Tasks;
using GoferEx.ExternalResources;
using Microsoft.AspNetCore.Http;

namespace GoferEx.Server.Helpers.Interfaces
{
    public interface IAuthTokenFactory
    {
        Task<BaseOAuthToken> CreateAuthToken(HttpContext context);
    }
}