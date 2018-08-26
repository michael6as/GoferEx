using GoferEx.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoferEx.Storage
{
    public interface IDbProvider
    {
        List<Contact> GetContacts();
        Contact GetContact(Guid id);
        bool RemoveContact(Guid id);
        bool AddContacts(List<Contact> contacts);        
        bool UpdateContacts(List<Contact> contacts);
    }
}
