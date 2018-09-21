using GoferEx.Core;
using GoferEx.ExternalResources;
using GoferEx.Server.Helpers.Interfaces;
using GoferEx.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.ExternalResources.Abstract;

namespace GoferEx.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IAuthTokenFactory _factory;
        private IDataHandler _handler;
        public ContactController(IDataHandler handler, IAuthTokenFactory factory)
        {
            _handler = handler;
            _factory = factory;
            //_dbProvider = new RedisDbProvider("localhost");
            //_handler = new GoogleResourceHandler();
            //_factory = new SingleAuthTokenFactory();
        }

        [HttpGet()]
        public async Task<SyncContactObject> Get()
        {
            try
            {
                var token = await _factory.CreateAuthToken(HttpContext);
                return new SyncContactObject(token.ResourceProvider, await _handler.GetContacts(token));
            }
            catch (Exception e)
            {
                throw CreateClientMessage(e, "Error Occured while getting contacts from an external provider");
            }
        }

        [HttpPost()]
        public async Task<IEnumerable<Contact>> Post([FromBody]Contact[] contacts)
        {
            try
            {
                var token = await _factory.CreateAuthToken(HttpContext);
                await _handler.UpdateContacts(token, contacts.ToList(), false);
                return await _handler.GetContacts(token);
            }
            catch (Exception e)
            {
                throw CreateClientMessage(e, "Error Occured while changing contacts");
            }
        }

        [HttpDelete()]
        public async Task<IEnumerable<Contact>> Delete([FromBody]Contact contact)
        {
            try
            {
                var token = await _factory.CreateAuthToken(HttpContext);
                var contactList = new List<Contact>() { contact };
                await _handler.DeleteContacts(token, contactList, false);
                return await _handler.GetContacts(token);
            }
            catch (Exception e)
            {
                throw CreateClientMessage(e, "Error Occured while changing contacts");
            }
        }        

        /// <summary>
        /// Create an error message to display for user.
        /// The client should get an error obj instead of an exception
        /// </summary>
        /// <param name="e"></param>
        /// <param name="userMsg"></param>
        /// <returns></returns>
        private Exception CreateClientMessage(Exception e, string userMsg)
        {
            return new Exception($"{userMsg} Exception Details: {e.Message}");
        }
    }
}
