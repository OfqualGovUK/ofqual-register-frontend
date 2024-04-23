namespace Ofqual.Common.RegisterFrontend.Models.SearchViewModels
{
    public class OrganisationListViewModel
    {
        public required string Name { get; set; }

        public required string RecognitionNumber { get; set; }

        public required string Acronym { get; set; }

        public string? OfqualOrganisationStatus { get; set; }

        public string? CceaOrganisationStatus { get; set; }
    }
}
