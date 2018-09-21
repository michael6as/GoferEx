using GoferEx.Core;
using GoferEx.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.ExternalResources.Abstract;

namespace GoferEx.ExternalResources
{
    /// <summary>
    /// The basic implementation for handling data from both local & external resources
    /// </summary>
    public class BasicDataHandler : IDataHandler
    {
        private IDictionary<string, IResourceHandler> _resourceHandlers;
        private IDbProvider _dbProvider;

        public BasicDataHandler(IDictionary<string, IResourceHandler> resourceHandlers, IDbProvider dbProvider)
        {
            _resourceHandlers = resourceHandlers;
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<Contact>> GetContacts(ResourceAuthToken token)
        {

            var dbContacts = await _dbProvider.GetContacts(token.Id);
            if (token.AuthToken != null && _resourceHandlers.ContainsKey(token.ResourceProvider))
            {
                var externalContacts = _resourceHandlers[token.ResourceProvider].RetrieveContacts(token);
                var newContacts = externalContacts.Where(externalContact => dbContacts.Any(dbContact => dbContact.Id != externalContact.Id)).ToList();
                await _dbProvider.UpdateContacts(token.Id, newContacts);
            }

            dbContacts = await _dbProvider.GetContacts(token.Id);
            return dbContacts.ToList();
        }

        public async Task<IEnumerable<Contact>> UpdateContacts(ResourceAuthToken token, IEnumerable<Contact> updatedContacts, bool changeInResource = false)
        {
            var contactsList = updatedContacts.ToList();
            await _dbProvider.UpdateContacts(token.Id, contactsList.ToList());
            if (changeInResource && _resourceHandlers.ContainsKey(token.ResourceProvider))
            {
                _resourceHandlers[token.ResourceProvider].AddContacts(token, contactsList);
            }

            return await _dbProvider.GetContacts(token.Id);
        }

        public async Task<IEnumerable<Contact>> DeleteContacts(ResourceAuthToken token, IEnumerable<Contact> updatedContacts, bool changeInResource = false)
        {
            var contactsList = updatedContacts.ToList();
            foreach (var updatedContact in contactsList)
            {
                await _dbProvider.RemoveContact(token.Id, updatedContact);
            }
            if (changeInResource && _resourceHandlers.ContainsKey(token.ResourceProvider))
            {
                _resourceHandlers[token.ResourceProvider].DeleteContacts(token, contactsList);
            }
            return await _dbProvider.GetContacts(token.Id);
        }
    }
}