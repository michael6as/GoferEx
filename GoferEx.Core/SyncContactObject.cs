using System.Collections.Generic;

namespace GoferEx.Core
{
    public class SyncContactObject
    {
        public string Provider { get; }
        public IEnumerable<Contact> RetrievedContacts { get; }

        public SyncContactObject(string provider, IEnumerable<Contact> retrievedContacts)
        {
            Provider = provider;
            RetrievedContacts = retrievedContacts;
        }
    }
}