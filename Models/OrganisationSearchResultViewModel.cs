namespace Ofqual.Common.RegisterFrontend.Models
{
    public class OrganisationSearchResultViewModel
    {
        public string? Name {  get; set; }
        public required APIResponseList<OrganisationListViewModel> List { get; set; }
    }
}
