using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Utilities
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public bool IsCurrentPage(int pageIndex)
        {
            return PageIndex == pageIndex;
        }

        public IEnumerable<int> GetPageIndexes(int pageSum)
        {
            int preSpan;
            int nextSpan;
            if (pageSum % 2 == 0)
            {
                preSpan = pageSum / 2 - 1;
                nextSpan = pageSum / 2;
            }
            else
            {
                preSpan = (pageSum - 1) / 2;
                nextSpan = (pageSum - 1) / 2;
            }
            int firstIndex = (PageIndex - preSpan > 1) ? (PageIndex - preSpan) : 1;
            int lastInedx = (PageIndex + nextSpan < TotalPages) ? (PageIndex + nextSpan) : TotalPages;
            return Enumerable.Range(firstIndex, lastInedx);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
