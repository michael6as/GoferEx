using GoferEx.Core;
using GoferEx.ExternalResources.Abstract;
using GoferEx.Server.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var token = await _factory.CreateAuthToken(HttpContext);
            var query = Request.Query;

            if (query.Count <= 0)
                return await _handler.GetContacts(token);

            var contactId = query["contactId"];
            var contactRes = await _handler.GetContact(token, contactId);
            return contactRes;
        }

        [HttpPost()]
        public async Task<SyncContactObject> Post([FromBody]Contact contact)
        {
            try
            {
                var token = await _factory.CreateAuthToken(HttpContext);
                await _handler.UpdateContacts(token, new List<Contact>() { contact }, false);
                return await _handler.GetContacts(token);
            }
            catch (Exception e)
            {
                return new SyncContactObject(new List<Contact>(), "", new ErrorMessage("Error occurred while authorizing the token", e));
            }
        }

        [HttpDelete()]
        public async Task<SyncContactObject> Delete()
        {
            try
            {
                var contactId = Request.Query["contactId"];
                var token = await _factory.CreateAuthToken(HttpContext);                
                await _handler.DeleteContact(token, contactId, false);
                return await _handler.GetContacts(token);
            }
            catch (Exception e)
            {
                return new SyncContactObject(new List<Contact>(), "", new ErrorMessage("Error occurred while authorizing the token", e));
            }
        }
    }
}
