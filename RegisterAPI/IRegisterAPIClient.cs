using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
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


        [Get("/api/qualifications?title={title}&page={page}&limit={limit}&assessmentMethods={assessmentMethods}&gradingTypes={gradingTypes}&awardingOrganisations={awardingOrganisations}&availability={availability}&qualificationTypes={qualificationTypes}&qualificationLevels={qualificationLevels}&qualificationSubLevels={qualificationSubLevels}&nationalAvailability={nationalAvailability}&minTotalQualificationTime={minTotalQualificationTime}&maxTotalQualificationTime={maxTotalQualificationTime}&minGuidedLearninghours={minGuidedLearninghours}&maxGuidedLearninghours={maxGuidedLearninghours}&sectorSubjectAreas={sectorSubjectAreas}")]
        Task<APIResponseList<QualificationListViewModel>> GetQualificationsListAsync(string? title, int page, int limit, string? assessmentMethods, string? gradingTypes, string? awardingOrganisations, string? availability, string? qualificationTypes, string? qualificationLevels, string? qualificationSubLevels, string? nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours, string? sectorSubjectAreas);
    }
}
