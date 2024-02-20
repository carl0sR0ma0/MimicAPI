using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Repositories.Contracts;

namespace MimicAPI.V1.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly MimicContext _database;

        public WordRepository(MimicContext database)
        {
            _database = database;
        }
        public PaginationList<Word> GetAll(WordUrlQuery query)
        {
            var list = new PaginationList<Word>();

            var item = _database.Words.AsNoTracking().AsQueryable();

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
                pagination.TotalPages = (int)Math.Ceiling((double)quantityTotalRegister / query.PerPage.Value);

                list.Pagination = pagination;
            }
            list.Results.AddRange(item.ToList());
            return list;
        }

        public Word GetById(int id)
        {
            return _database.Words.AsNoTracking().FirstOrDefault(w => w.Id == id);
        }

        public void Insert(Word word)
        {
            _database.Words.Add(word);
            _database.SaveChanges();
        }

        public void Update(Word word)
        {
            _database.Words.Update(word);
            _database.SaveChanges();
        }

        public void Delete(int id)
        {
            var word = GetById(id);
            word.Active = false;
            _database.Words.Update(word);
            _database.SaveChanges();
        }
    }
}
