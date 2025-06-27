namespace Postkit.API.Helpers
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}