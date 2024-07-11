using CsvHelper.Configuration.Attributes;
using Microsoft.Identity.Client;

namespace Ofqual.Common.RegisterFrontend.Models
{
    public class QualificationSitemap
    {
        public int Count {  get; set; }
        public List<QualificationSitemapData> Qualifications { get; set; } = [];
        public required PagingModel Paging { get; set; } = new PagingModel();
    }

    public class QualificationSitemapData
    {
        public required string QualificationNumber { get; set; }
        public required string QualificationNumberNoObliques { get; set; }
        public required string Title { get; set; }
    }
}
