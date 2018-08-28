using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.Core;
using GoferEx.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoferEx.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ContactController : ControllerBase
  {
    private IDbProvider _dbProvider;
    public ContactController() : base()
    {
      _dbProvider = new FileSystemProvider(@"F:\Gof\cts", @"F:\Gof\imgs");
    }

    [HttpGet()]
    public List<Contact> Get()
    {
      return _dbProvider.GetContacts().Result.ToList();
    }

    [HttpGet("{id}")]
    public Contact Get(string id)
    {
      return _dbProvider.GetContact(Guid.Parse(id)).Result;
    }

    [HttpPost()]
    public List<Contact> Post([FromBody]Contact contact)
    {
      var contactList = new List<Contact>() { contact };
      if (_dbProvider.GetContact(Guid.Parse(contact.Id.ToString())) != null)
      {
        _dbProvider.UpdateContacts(contactList);
      }
      _dbProvider.AddContacts(contactList);
      return _dbProvider.GetContacts().Result.ToList();
    }

    [HttpDelete("{id}")]
    public List<Contact> Delete(string id)
    {
      if (_dbProvider.RemoveContact(Guid.Parse(id)).Result)
      {
        var contacts = _dbProvider.GetContacts().Result;
        if (contacts != null)
        {
          return _dbProvider.GetContacts().Result.ToList();
        }        
      }

      return null;
    }
  }
}