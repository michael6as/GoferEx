using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace GoferEx.Server.Helpers.Interfaces
{
    public interface IAuthProvider
    {
        void AddAuthMiddleware(AuthenticationBuilder services);
    }
}