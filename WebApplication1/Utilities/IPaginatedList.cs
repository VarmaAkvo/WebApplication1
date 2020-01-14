using System.Collections.Generic;

namespace WebApplication1.Utilities
{
    public interface IPaginatedList
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int PageIndex { get; }
        int TotalPages { get; }

        IEnumerable<int> GetPageIndexes(int pageSum);
        bool IsCurrentPage(int pageIndex);
    }
}