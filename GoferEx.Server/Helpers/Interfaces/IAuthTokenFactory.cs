using System.Threading.Tasks;
using GoferEx.Core;
using GoferEx.ExternalResources;
using Microsoft.AspNetCore.Http;

namespace GoferEx.Server.Helpers.Interfaces
{
    /// <summary>
    /// Factory to get the oauth token for the providers.    
    /// </summary>
    public interface IAuthTokenFactory
    {
        Task<ResourceAuthToken> CreateAuthToken(HttpContext context);
    }
}