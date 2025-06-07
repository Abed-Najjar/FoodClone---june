namespace API.DTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PagedResultDto()
        {
        }

        public PagedResultDto(List<T> data, int totalItems, int pageNumber, int pageSize)
        {
            Data = data;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            HasNextPage = pageNumber < TotalPages;
            HasPreviousPage = pageNumber > 1;
        }
    }
}
