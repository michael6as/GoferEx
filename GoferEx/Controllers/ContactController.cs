using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GoferEx.Core;
using GoferEx.Storage;
using Google.GData.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<List<Contact>> Get()
    {
      var context = this.HttpContext;
      var oauthParams = new OAuth2Parameters()
      {
        AccessToken = await context.GetTokenAsync("access_token"),
        AccessCode = await context.GetTokenAsync("code"),
        AccessType = "offline",
        RefreshToken = await context.GetTokenAsync("refresh_token"),
        TokenExpiry = DateTime.Parse(await context.GetTokenAsync("expires_at")),
        Scope = await context.GetTokenAsync("scope")

      };
      return null;
      var contacts = _dbProvider.GetContacts().Result;
      if (contacts != null)
      {
        return contacts.ToList();
      }
      return null;
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
      else
      {
        _dbProvider.AddContacts(contactList);
      }      
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
