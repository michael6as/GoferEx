using System.Collections.Generic;

namespace GoferEx.Core
{
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