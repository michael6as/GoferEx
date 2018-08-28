using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace GoferEx.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SyncController : ControllerBase
  {
    [HttpGet()]
    public void Get(string code)
    {
      ContactResourceHandler contactHandler = new ContactResourceHandler(code);
      contactHandler.ValidateToken(new CustomGoogleToken(code, "Google"));

    }

    // Instead of direct initializing and of course, OCP, need to change to a
    // dictionary with Provider as the key and the Handler object as the Value
    [HttpPost()]
    public List<Contact> Post(ResourceContext context)
    {
      ContactResourceHandler contactHandler = new ContactResourceHandler(context.RedirectUri);
      contactHandler.Authenticate(context.Scopes);
      return contactHandler.RetrieveContacts(new CustomGoogleToken(context.Token, context.Provider)).ToList();
    }
  }
}
