using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.Core;
using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using GoferEx.Server.Helpers.Interfaces;
using GoferEx.Server.Helpers.TokenFactory;
using GoferEx.Storage;
using Google.GData.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GoferEx.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IDbProvider _dbProvider;
        private IResourceHandler _handler;
        private IAuthTokenFactory _factory;
        public ContactController() : base()
        {
            try
            {                
                _dbProvider = new RedisDbProvider("localhost");
                _handler = new GoogleResourceHandler();
                _factory = new SingleAuthTokenFactory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }            
        }

        [HttpGet()]
        public async Task<List<Contact>> Get()
        {
            try
            {
                var oauthParams = await _factory.CreateAuthToken(this.HttpContext);
                var contacts = _handler.RetrieveContacts(oauthParams.AuthToken);
                return contacts.ToList();
            }
            catch (Exception e)
            {
                return null;
            }            
        }

        //[HttpGet("{id}")]
        //public Contact Get(string id)
        //{
        //    return _dbProvider.GetContact(Guid.Parse(id)).Result;
        //}

        //[HttpPost()]
        //public List<Contact> Post([FromBody]Contact contact)
        //{
        //    var contactList = new List<Contact>() { contact };
        //    if (_dbProvider.GetContact(Guid.Parse(contact.Id.ToString())) != null)
        //    {
        //        _dbProvider.UpdateContacts(contactList);
        //    }
        //    else
        //    {
        //        _dbProvider.AddContacts(contactList);
        //    }
        //    return _dbProvider.GetContacts().Result.ToList();
        //}

        //[HttpDelete("{id}")]
        //public List<Contact> Delete(string id)
        //{
        //    if (_dbProvider.RemoveContact(Guid.Parse(id)).Result)
        //    {
        //        var contacts = _dbProvider.GetContacts().Result;
        //        if (contacts != null)
        //        {
        //            return _dbProvider.GetContacts().Result.ToList();
        //        }
        //    }

        //    return null;
        //}
    }
}
