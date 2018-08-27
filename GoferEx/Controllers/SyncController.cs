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
      Class1 c = new Class1("https://accounts.google.com/o/oauth2/token", "535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com", "Mk8mvSz3SmHmi9Enz19vsPbX", "http://localhost:1906/signin-google", "https://www.googleapis.com/oauth2/v1/certs");
      c.ExchangeCodeForTokens(context.Token);
      //GoogleContactProvider prov = new GoogleContactProvider("535817455358-ssrvfaf6ml34hq7h54gqt8f1eunlr5mm.apps.googleusercontent.com", "DsZdQljO6RddI87RcQCVu2dB");
      //prov.GetAccessToken(context.Token);
      //prov.Con();
      //prov.GetS();
      var co1 = new Contact(Guid.NewGuid().ToString(), "Synced", "1", "SyncedHell", "1@Synced.com", DateTime.Now, "19061906", "Synced");
      var co2 = new Contact(Guid.NewGuid().ToString(), "Synced2", "12", "SyncedHell2", "1@Synced.com", DateTime.Now, "19061906", "Synced");
      return new List<Contact>() { co1, co2 };
    }
  }
}
