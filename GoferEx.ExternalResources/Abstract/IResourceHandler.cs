using GoferEx.Core;
using System.Collections.Generic;

namespace GoferEx.ExternalResources
{
    public interface IResourceHandler<in T> where T : BaseToken
    {
        void Authenticate(string[] scopes);
        bool ValidateToken(T token);
        IEnumerable<Contact> RetrieveContacts(T token);
    }
}