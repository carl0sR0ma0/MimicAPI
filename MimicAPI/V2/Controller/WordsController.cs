using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class WordsController : ControllerBase
    {
        [HttpGet(Name = "GetAll")]
        public IActionResult GetWords()
        {
            return Ok("Version 2.0");
        }
    }
}
