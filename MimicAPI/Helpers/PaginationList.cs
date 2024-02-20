using MimicAPI.V1.Models.DTO;

namespace MimicAPI.Helpers
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public Pagination? Pagination { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
