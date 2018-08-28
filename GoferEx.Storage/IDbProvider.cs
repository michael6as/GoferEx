using GoferEx.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoferEx.Storage
{
    public interface IDbProvider
    {
        Task<IEnumerable<Contact>> GetContacts();
        Task<Contact> GetContact(Guid id);
        Task<bool> RemoveContact(Guid id);
        Task<bool> AddContacts(List<Contact> contacts);        
        Task<bool> UpdateContacts(List<Contact> contacts);
    }
}
