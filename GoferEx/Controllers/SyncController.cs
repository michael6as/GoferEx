using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;

namespace GoferEx.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SyncController : ControllerBase
  {
    public SyncController() : base()
    {
      //TODO: Add sync dict
    }

    [HttpPost()]
    public List<Contact> Post([FromBody])
    {
      GoogleContactProvider prov = new GoogleContactProvider("535817455358-ssrvfaf6ml34hq7h54gqt8f1eunlr5mm.apps.googleusercontent.com", "DsZdQljO6RddI87RcQCVu2dB");
      prov.Con();
      prov.GetS();
      var co1 = new Contact(Guid.NewGuid().ToString(), "Synced", "1", "SyncedHell", "1@Synced.com", DateTime.Now, "19061906", "Synced");
      var co2 = new Contact(Guid.NewGuid().ToString(), "Synced2", "12", "SyncedHell2", "1@Synced.com", DateTime.Now, "19061906", "Synced");
      return new List<Contact>() { co1, co2 };
    }
  }
}
