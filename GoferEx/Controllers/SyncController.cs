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

    [HttpPost()]
    public List<Contact> Post(ResourceContext context)
    {
      ContactResourceHandler contactHandler = new ContactResourceHandler(context.RedirectUri);
      contactHandler.Authenticate(context.Scopes);
      return contactHandler.RetrieveContacts(new CustomGoogleToken(context.Token, context.Provider)).ToList();
    }
  }
}
