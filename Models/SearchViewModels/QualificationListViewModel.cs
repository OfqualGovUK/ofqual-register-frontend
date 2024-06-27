namespace Ofqual.Common.RegisterFrontend.Models.SearchViewModels
{
    public class QualificationListViewModel
    {
        public required string Title { get; set; }

        public required string QualificationNumber { get; set; }
        public required string QualificationNumberNoObliques { get; set; }

        public required string? Status { get; set; }

        public required string OrganisationName { get; set; }

        public string? Level { get; set; }
    }
}
