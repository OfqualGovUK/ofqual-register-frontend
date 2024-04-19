namespace Ofqual.Common.RegisterFrontend.Models
{
    public class APIResponseList<T>
    {
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        public int? Limit { get; set; }

        public List<T>? Results { get; set; } = [];
    }
}
