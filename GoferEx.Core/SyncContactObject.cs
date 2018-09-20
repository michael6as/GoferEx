using System.Collections.Generic;

namespace GoferEx.Core
{
    public class SyncContactObject
    {
        public string Provider { get; private set; }
        public IEnumerable<Contact> RetrievedContacts { get; private set; }

        public SyncContactObject(string provider, IEnumerable<Contact> retrievedContacts)
        {
            Provider = provider;
            RetrievedContacts = retrievedContacts;
        }
    }
}