using CustomEd.Shared.Response;

namespace CusotmEd.Forum.Responses
{
    public class PaginatedResponse<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
