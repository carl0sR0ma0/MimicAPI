using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using Newtonsoft.Json;
using System.Linq;

namespace MimicAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly MimicContext _database;

        public WordsController(MimicContext database)
        {
            _database = database;
        }

        [HttpGet]
        public IActionResult GetWords([FromQuery] WordUrlQuery query)
        {
            var item = _database.Words.AsQueryable();

            if (query.Date.HasValue)
            {
                item = item.Where(w => w.CreateAt > query.Date.Value || w.UpdateAt > query.Date.Value);
            }

            if (query.NumberPage.HasValue)
            {
                var quantityTotalRegister = item.Count();

                item = item.Skip((query.NumberPage.Value - 1) * query.PerPage.Value).Take(query.PerPage.Value);

                var pagination = new Pagination();
                pagination.NumberPage = query.NumberPage.Value;
                pagination.PerPage = query.PerPage.Value;
                pagination.Total = quantityTotalRegister;
                pagination.TotalPages = (int) Math.Ceiling((double) quantityTotalRegister / query.PerPage.Value);

                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(pagination));

                if (query.NumberPage > pagination.TotalPages) return NotFound();
            }

            return Ok(item);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetWord(int id)
        {
            var result = _database.Words.Find(id);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Register([FromBody] Word word)
        {
            _database.Words.Add(word);
            _database.SaveChanges();

            return Created($"/api/words/{word.Id}", word);
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Update(int id, [FromBody] Word word)
        {
            var result = _database.Words.AsNoTracking().FirstOrDefault(w => w.Id == id);
            if (result is null) return NotFound();

            word.Id = id;
            _database.Words.Update(word);
            _database.SaveChanges();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var word = _database.Words.Find(id);
            if (word is null) return NotFound(); 

            word.Active = false;
            _database.Words.Update(word);
            _database.SaveChanges();

            return NoContent();
        }
    }
}
