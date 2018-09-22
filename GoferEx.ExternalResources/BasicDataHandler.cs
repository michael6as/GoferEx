using GoferEx.Core;
using GoferEx.ExternalResources.Abstract;
using GoferEx.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.GData.Client;

namespace GoferEx.ExternalResources
{
    /// <summary>
    /// The basic implementation for handling data from both local & external resources
    /// </summary>
    public class BasicDataHandler : IDataHandler
    {
        private readonly IDictionary<string, IResourceHandler> _resourceHandlers;
        private readonly IDbProvider _dbProvider;

        public BasicDataHandler(IDictionary<string, IResourceHandler> resourceHandlers, IDbProvider dbProvider)
        {
            _resourceHandlers = resourceHandlers;
            _dbProvider = dbProvider;
        }

        public async Task<SyncContactObject> GetContacts(ResourceAuthToken token)
        {

            var dbContacts = await _dbProvider.GetContacts(token.Id);
            if (token.AuthToken == null || !_resourceHandlers.ContainsKey(token.ResourceProvider))
            {
                return new SyncContactObject(dbContacts.ToList(), token.ResourceProvider);
            }

            try
            {
                var externalContacts = _resourceHandlers[token.ResourceProvider].RetrieveContacts(token);
                var newContacts = externalContacts.Where(externalContact =>
                    dbContacts.All(dbContact => dbContact.Id != externalContact.Id));                    
                await _dbProvider.UpdateContacts(token.Id, newContacts.ToList());
                dbContacts = await _dbProvider.GetContacts(token.Id);
            }
            catch (GDataRequestException e)
            {
                var errMsg = new ErrorMessage($"Error occurred while retrieving data from {token.ResourceProvider}. Refresh token",
                    e);
                return new SyncContactObject(dbContacts, token.ResourceProvider, errMsg);
            }
            catch (Exception e)
            {
                var errMsg = new ErrorMessage($"General error occurred",
                    e);
                return new SyncContactObject(dbContacts, token.ResourceProvider, errMsg);                    
            }
            return new SyncContactObject(dbContacts.ToList(), token.ResourceProvider);
        }

        public async Task<SyncContactObject> GetContact(ResourceAuthToken token, string id)
        {
            var dbContacts = await _dbProvider.GetContacts(token.Id);
            return new SyncContactObject(new List<Contact>()
                {dbContacts.FirstOrDefault(dbContact => HttpUtility.UrlDecode(dbContact.Id) == id)},token.ResourceProvider);
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

        public async Task<IEnumerable<Contact>> DeleteContact(ResourceAuthToken token, string contactId, bool changeInResource = false)
        {
            await _dbProvider.RemoveContact(token.Id,
                (await GetContact(token, contactId)).RetrievedContacts.FirstOrDefault());
            if (changeInResource && _resourceHandlers.ContainsKey(token.ResourceProvider))
            {
                _resourceHandlers[token.ResourceProvider].DeleteContact(token, contactId);
            }
            return await _dbProvider.GetContacts(token.Id);
        }
    }
}