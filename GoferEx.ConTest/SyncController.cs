using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace GoferEx.ConTest
{
    [Route("api/[controller]")]
    [ApiController]    
    public class SyncController : ControllerBase
    {
        [HttpGet()]
        public void Get(string code)
        {            
        }
    }
}
