using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using API.Entities;

namespace API.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly DatabaseContext _context;

        public ValuesController(IHostingEnvironment env , DatabaseContext context)
        {
            _env = env;
            _context = context;
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                env = _env.EnvironmentName,
                ReleaseDate = "2018-06-07"
            });
        }

        // GET api/values/5
        [Authorize (Roles ="Admin")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "This is OK";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
