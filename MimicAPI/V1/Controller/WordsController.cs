using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Contracts;
using Newtonsoft.Json;

namespace MimicAPI.V1.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class WordsController : ControllerBase
    {
        private readonly IWordRepository _repository;
        private readonly IMapper _mapper;

        public WordsController(IWordRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetWords([FromQuery] WordUrlQuery query)
        {
            var item = _repository.GetAll(query);
            if (item.Results.Count() == 0) return NotFound();

            PaginationList<WordDTO> list = CreateLinksToListWordDTO(query, item);

            return Ok(list);
        }

        private PaginationList<WordDTO> CreateLinksToListWordDTO(WordUrlQuery query, PaginationList<Word> item)
        {
            var list = _mapper.Map<PaginationList<Word>, PaginationList<WordDTO>>(item);

            foreach (var word in list.Results)
            {
                word.Links.Add(new LinkDTO("self", Url.Link("GetWord", new { id = word.Id }), "GET"));
            }

            list.Links.Add(new LinkDTO("self", Url.Link("GetAll", query), "GET"));

            if (item.Pagination is not null)
            {
                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(item.Pagination));

                object? queryString = null;

                if (query.NumberPage + 1 <= item.Pagination.TotalPages)
                {
                    queryString = new WordUrlQuery() { NumberPage = query.NumberPage + 1, PerPage = query.PerPage, Date = query.Date };
                    list.Links.Add(new LinkDTO("next", Url.Link("GetAll", queryString), "GET"));
                }
                else if (query.NumberPage - 1 > 0)
                {
                    queryString = new WordUrlQuery() { NumberPage = query.NumberPage - 1, PerPage = query.PerPage, Date = query.Date };
                    list.Links.Add(new LinkDTO("prev", Url.Link("GetAll", queryString), "GET"));
                }

            }

            return list;
        }

        [HttpGet("{id}", Name = "GetWord")]
        public IActionResult GetWord(int id)
        {
            var result = _repository.GetById(id);
            if (result is null) return NotFound();

            WordDTO wordDTO = _mapper.Map<WordDTO>(result);
            wordDTO.Links.Add(
                new LinkDTO("self", Url.Link("GetWord", new { id = wordDTO.Id }), "GET")
            );
            wordDTO.Links.Add(
                new LinkDTO("update", Url.Link("UpdateWord", new { id = wordDTO.Id }), "PUT")
            );
            wordDTO.Links.Add(
                new LinkDTO("delete", Url.Link("DeleteWord", new { id = wordDTO.Id }), "DELETE")
            );

            return Ok(wordDTO);
        }

        [HttpPost]
        public IActionResult Register([FromBody] Word word)
        {
            if (word is null) BadRequest();
            if (!ModelState.IsValid) UnprocessableEntity(ModelState);

            word.Active = true;
            word.CreateAt = DateTime.Now;
            _repository.Insert(word);

            WordDTO wordDTO = _mapper.Map<WordDTO>(word);
            wordDTO.Links.Add(
                new LinkDTO("self", Url.Link("GetWord", new { id = wordDTO.Id }), "GET")
            );

            return Created($"/api/words/{word.Id}", word);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("{id}", Name = "UpdateWord")]
        public IActionResult Update(int id, [FromBody] Word word)
        {
            var result = _repository.GetById(id);

            if (result is null) return NotFound();
            if (word is null) BadRequest();
            if (!ModelState.IsValid) UnprocessableEntity(ModelState);

            word.Id = id;
            word.Active = result.Active;
            word.CreateAt = result.CreateAt;
            word.UpdateAt = DateTime.Now;
            _repository.Update(word);

            WordDTO wordDTO = _mapper.Map<WordDTO>(word);
            wordDTO.Links.Add(
                new LinkDTO("self", Url.Link("GetWord", new { id = wordDTO.Id }), "GET")
            );

            return Ok();
        }

        [HttpDelete("{id}", Name = "DeleteWord")]
        public IActionResult Delete(int id)
        {
            var word = _repository.GetById(id);
            if (word is null) return NotFound();

            _repository.Delete(id);

            return NoContent();
        }
    }
}
