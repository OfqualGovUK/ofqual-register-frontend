namespace Ofqual.Common.RegisterFrontend.Models
{
    public class PagingModel
    {
        public string? PagingURL { get; set; }
        public List<int>? PagingList { get; set; }
        public int? CurrentPage { get; set; }
    }
}
