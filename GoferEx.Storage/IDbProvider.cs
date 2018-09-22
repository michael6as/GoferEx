using GoferEx.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoferEx.Storage
{
    public interface IDbProvider
    {
        Task<IList<Contact>> GetContacts(string id);
        Task<bool> RemoveContact(string id, Contact contact);       
        Task<bool> UpdateContacts(string id, List<Contact> contacts);
    }
}
