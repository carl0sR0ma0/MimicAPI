using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Contracts
{
    public interface IWordRepository
    {
        PaginationList<Word> GetAll(WordUrlQuery query);
        Word GetById(int id);
        void Insert(Word word);
        void Update(Word word);
        void Delete(int id);
    }
}
