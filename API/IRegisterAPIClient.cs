using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Refit;
using System.Net;

namespace Ofqual.Common.RegisterFrontend.RegisterAPI
{
    public interface IRegisterAPIClient
    {
        [Get("/api/organisations/{recognitionNumber}")]
        Task<Organisation> GetOrganisationAsync(string recognitionNumber);

        [Get("/api/organisations?search={search}&page={page}&limit={limit}")]
        Task<APIResponseList<OrganisationListViewModel>> GetOrganisationsListAsync(string? search, int page, int limit);


        [Get("/api/qualifications?title={title}&page={page}&limit={limit}&assessmentMethods={assessmentMethods}&gradingTypes={gradingTypes}&awardingOrganisations={awardingOrganisations}&availability={availability}&qualificationTypes={qualificationTypes}&qualificationLevels={qualificationLevels}&nationalAvailability={nationalAvailability}&sectorSubjectAreas={sectorSubjectAreas}")]
        Task<APIResponseList<QualificationListViewModel>> GetQualificationsListAsync(string? title, int page, int limit, string? assessmentMethods = null, string? gradingTypes = null, string? awardingOrganisations = null, string? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, string? sectorSubjectAreas = null);
    }
}
