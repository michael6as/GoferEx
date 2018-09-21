using GoferEx.Core;
using System.Collections.Generic;
using Google.GData.Client;

namespace GoferEx.ExternalResources
{
    public interface IResourceHandler
    {
        IEnumerable<Contact> RetrieveContacts(ResourceAuthToken authParams);

        bool AddContacts(ResourceAuthToken authParams, IEnumerable<Contact> contacts);
        bool DeleteContacts(ResourceAuthToken authParams, IEnumerable<Contact> contacts);
    }
}