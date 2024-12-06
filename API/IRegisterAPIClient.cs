using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV;
using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
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

        [Get("/api/organisations?search={search}")]
        Task<APIResponseList<Organisation>> GetOrganisationsDetailListAsync(string? search);


        [Get("/api/organisations")]
        Task<APIResponseList<OrganisationCSV>> GetFullOrganisationsDataSetAsync();

        [Get("/api/scopes/{recognitionNumber}")]
        Task<RecognitionScope> GetOrganisationsScopes(string recognitionNumber);


        [Get("/api/qualifications/{number1}/{number2}/{number3}")]
        Task<Qualification> GetQualificationAsync(string number1, string? number2 = null, string? number3 = null);


        [Get("/api/qualifications?title={title}&page={page}&limit={limit}&assessmentMethods={assessmentMethods}&gradingTypes={gradingTypes}&availability={availability}&qualificationTypes={qualificationTypes}&qualificationLevels={qualificationLevels}&nationalAvailability={nationalAvailability}&minTotalQualificationTime={minTotalQualificationTime}&maxTotalQualificationTime={maxTotalQualificationTime}&minGuidedLearninghours={minGuidedLearninghours}&maxGuidedLearninghours={maxGuidedLearninghours}")]
        Task<APIResponseList<QualificationListViewModel>> GetQualificationsListAsync(string? title, int? page = null, int? limit = null, string? assessmentMethods = null, string? gradingTypes = null, [Query(CollectionFormat.Multi)] string[]? awardingOrganisations = null, string ? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, [Query(CollectionFormat.Multi)] string[]? sectorSubjectAreas = null, int? minTotalQualificationTime = null, int? maxTotalQualificationTime = null, int? minGuidedLearninghours = null, int? maxGuidedLearninghours = null);



        [Get("/api/qualifications?title={title}&page={page}&limit={limit}&assessmentMethods={assessmentMethods}&gradingTypes={gradingTypes}&availability={availability}&qualificationTypes={qualificationTypes}&qualificationLevels={qualificationLevels}&nationalAvailability={nationalAvailability}&minTotalQualificationTime={minTotalQualificationTime}&maxTotalQualificationTime={maxTotalQualificationTime}&minGuidedLearninghours={minGuidedLearninghours}&maxGuidedLearninghours={maxGuidedLearninghours}")]
        Task<APIResponseList<QualificationCSV>> GetFullQualificationsDataSetAsync(string? title, int? page = null, int? limit = null, string? assessmentMethods = null, string? gradingTypes = null, [Query(CollectionFormat.Multi)] string[]? awardingOrganisations = null, string? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, [Query(CollectionFormat.Multi)] string[]? sectorSubjectAreas = null, int? minTotalQualificationTime = null, int? maxTotalQualificationTime = null, int? minGuidedLearninghours = null, int? maxGuidedLearninghours = null);

        [Get("/api/qualifications")]
        Task<APIResponseList<QualificationSitemapData>> GetQualificationsForSitemap();
    }
}
