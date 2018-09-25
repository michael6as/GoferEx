using System.Collections.Generic;

namespace GoferEx.Core
{
    /// <summary>
    /// Model for representing the retrieved information from the external resources and DB
    /// </summary>
    public class SyncContactObject
    {
        public string Provider { get; }
        public IEnumerable<Contact> RetrievedContacts { get; }
        public ErrorMessage ServerError { get; }

        public SyncContactObject(IEnumerable<Contact> retrievedContacts, string provider = "", ErrorMessage msg = null)
        {
            Provider = provider;
            RetrievedContacts = retrievedContacts;
            ServerError = msg;
        }
    }
}