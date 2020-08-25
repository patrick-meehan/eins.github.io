using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EinsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Game : ControllerBase
    {
        // GET: api/<Game>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Game>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Game>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Game>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Game>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
