using System.Collections.Generic;
using System.Threading.Tasks;
using GoferEx.Core;

namespace GoferEx.ExternalResources.Abstract
{
    /// <summary>
    /// Interface for combining the data from the external resources with the Database
    /// </summary>
    public interface IDataHandler
    {
        Task<SyncContactObject> GetContacts(ResourceAuthToken token);
        Task<SyncContactObject> GetContact(ResourceAuthToken token, string id);
        Task<IEnumerable<Contact>> UpdateContacts(ResourceAuthToken token, IEnumerable<Contact> updatedContacts, bool changeInResource);
        Task<IEnumerable<Contact>> DeleteContact(ResourceAuthToken token, string contactId, bool changeInResource);
    }
}