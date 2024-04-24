namespace Ofqual.Common.RegisterFrontend.Models.SearchViewModels
{
    public class SearchResultViewModel<T>
    {
        public string? Title { get; set; }
        public required APIResponseList<T> List { get; set; }
        public required PagingModel Paging { get; set; }

        public FilterModel? Filters { get; set; }
        public FilterModel? AppliedFilters { get; set; }
    }
}
