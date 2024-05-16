namespace Ofqual.Common.RegisterFrontend.Models
{
    //generic class for any results from the register API 
    public class APIResponseList<T>
    {
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        public int? Limit { get; set; }

        //the actual search results
        public List<T>? Results { get; set; } = [];
    }
}
