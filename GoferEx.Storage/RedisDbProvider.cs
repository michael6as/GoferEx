using GoferEx.Core;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace GoferEx.Storage
{
    public class RedisDbProvider : IDbProvider
    {
        private ConnectionMultiplexer _redisConn;
        public RedisDbProvider(string redisHost)
        {
            _redisConn = ConnectionMultiplexer.Connect(redisHost);
        }

        public async Task<IList<Contact>> GetContacts(string id)
        {
            HashEntry[] entries = await _redisConn.GetDatabase().HashGetAllAsync(id);

            return entries.Select(hashEntry => JsonConvert.DeserializeObject<Contact>(hashEntry.Value)).ToList();
        }

        public async Task<bool> RemoveContact(string id, Contact contact)
        {
            return await _redisConn.GetDatabase().HashDeleteAsync(id, contact.Id.ToString());
        }

        public async Task<bool> UpdateContacts(string id, List<Contact> contacts)
        {
            IDatabase db = _redisConn.GetDatabase();            
            List<Contact> errorContacts = new List<Contact>();
            foreach (Contact contact in contacts)
            {
                if (!await db.HashSetAsync(id, contact.Id.ToString(), contact.ToString()))
                {
                    errorContacts.Add(contact);
                }
            }

            if (errorContacts.Count > 0)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);            
        }
    }
}