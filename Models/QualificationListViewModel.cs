namespace Ofqual.Common.RegisterFrontend.Models
{
    public class QualificationListViewModel
    {
        public required string Title { get; set; }

        public required string QualificationNumber { get; set; }

        public required string Status { get; set; }

        public required string OrganisationName { get; set; }

        public string? Level { get; set; }
    }
}
