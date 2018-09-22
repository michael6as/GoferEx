using System.Collections.Generic;
using GoferEx.Core;

namespace GoferEx.ExternalResources.Abstract
{
    public interface IResourceHandler
    {
        IEnumerable<Contact> RetrieveContacts(ResourceAuthToken authParams);

        bool AddContacts(ResourceAuthToken authParams, IEnumerable<Contact> contacts);
        bool DeleteContact(ResourceAuthToken authParams, string contactId);
    }
}