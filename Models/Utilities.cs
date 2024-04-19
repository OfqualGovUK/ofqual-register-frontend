using System.Reflection;

namespace Ofqual.Common.RegisterFrontend.Models
{
    public static class Utilities
    {
        public static List<int> GeneratePageList(int currentPage, int itemsCount, int limit)
        {
            List<int> pageList = new List<int>();
            var lastPage = itemsCount / limit;

            //for the last page if needed
            lastPage = itemsCount % limit > 0 ? lastPage + 1 : lastPage;

            const int maxPagesToShow = 3; // Number of pages to show in the list (excluding ellipses)

            int startPage = Math.Max(1, currentPage - (maxPagesToShow / 2));
            int endPage = Math.Min(lastPage, startPage + maxPagesToShow - 1);

            if (endPage - startPage + 1 < maxPagesToShow)
            {
                startPage = Math.Max(1, lastPage - maxPagesToShow + 1);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                pageList.Add(i);
            }

            if (startPage > 1)
            {
                pageList.Insert(0, 1); 
                if (startPage > 2)
                {
                    // Ellipses placeholder (-1 results in ... in the view)
                    pageList.Insert(1, -1); 
                }
            }

            if (endPage < lastPage)
            {
                if (endPage < lastPage - 1)
                {
                    pageList.Add(-1); 
                }
                pageList.Add(lastPage); 
            }

            return pageList;
        }
    }
}
