﻿namespace Ofqual.Common.RegisterFrontend.Models
{
    public class SearchResultViewModel<T>
    {
        public string? Name { get; set; }
        public required APIResponseList<T> List { get; set; }
        public string? PagingURL { get; set; }
        public List<int>? PagingList { get; set; }
    }
}