using GoferEx.Core;
using System.Collections.Generic;
using Google.GData.Client;

namespace GoferEx.ExternalResources
{
    public interface IResourceHandler
    {
        IEnumerable<Contact> RetrieveContacts(OAuth2Parameters authParamas);
    }
}