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
        /// <summary>
        /// Retrieves contacts from both DB and external resource
        /// </summary>
        /// <param name="token">The authorization token given by the user</param>
        /// <returns>The data retrieved from DB</returns>
        Task<SyncContactObject> GetContacts(ResourceAuthToken token);
        /// <summary>
        /// Get specific user information from the DB
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id">The Id of a given contact</param>
        /// <returns></returns>
        Task<SyncContactObject> GetContact(ResourceAuthToken token, string id);
        /// <summary>
        /// Update contacts in DB
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updatedContacts">The edited contacts </param>
        /// <param name="changeInResource">This is a flag for changing the data of a given contact at the authorized ext resources. default: false</param>
        /// <returns></returns>
        Task<IEnumerable<Contact>> UpdateContacts(ResourceAuthToken token, IEnumerable<Contact> updatedContacts, bool changeInResource);
        Task<IEnumerable<Contact>> DeleteContact(ResourceAuthToken token, string contactId, bool changeInResource);
    }
}