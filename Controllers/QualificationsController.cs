using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterAPI.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using System.Diagnostics;
using System.Xml.Linq;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class QualificationsController : Controller
    {
        private readonly ILogger<QualificationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IRefDataAPIClient _refDataAPIClient;
        private readonly IConfiguration _config;

        public QualificationsController(ILogger<QualificationsController> logger, IRegisterAPIClient registerAPIClient, IRefDataAPIClient refDataAPIClient, IConfiguration config)
        {
            _logger = logger;
            _registerAPIClient = registerAPIClient;
            _refDataAPIClient = refDataAPIClient;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string title, int page = 1, string? assessmentMethods = null, string? gradingTypes = null, string? awardingOrganisations = null, string? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, string? minTotalQualificationTime = null, int? maxTotalQualificationTime = null, int? minGuidedLearninghours = null, int? maxGuidedLearninghours = null, string? sectorSubjectAreas = null)
        {
            int pagingLimit = _config.GetValue<int>("QualificationsPagingLimit");

            var quals = await _registerAPIClient.GetQualificationsListAsync(title, page, pagingLimit, assessmentMethods: assessmentMethods, gradingTypes: gradingTypes, awardingOrganisations: awardingOrganisations, availability: availability, qualificationTypes: qualificationTypes, qualificationLevels: qualificationLevels, nationalAvailability: nationalAvailability);

            var ssa = await _refDataAPIClient.GetSSAAsync();
            var levels = await _refDataAPIClient.GetLevelsAsync();
            var assessMethods = await _refDataAPIClient.GetAssessmentMethodsAsync();
            var types = await _refDataAPIClient.GetQualificationTypesAsync();
            var organisations = await _registerAPIClient.GetOrganisationsListAsync(null, 1, 500);

            var model = new SearchResultViewModel<QualificationListViewModel>
            {
                List = quals,
                Title = title,
                Filters = new QualificationFilterModel
                {
                    AssessmentMethods = assessMethods.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                    QualificationLevels = levels.OrderBy(e => e.LevelDescription).Select(e => e.LevelDescription).ToHashSet(),
                    QualificationTypes = types.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                    SSA = ssa.OrderBy(e => e.SsaDescription2).Select(e => e.SsaDescription2).ToHashSet(),
                    Organisations = organisations.Results!.OrderBy(e => e.Name).Select(e => e.Name).ToHashSet()
                },
                Paging = new PagingModel
                {
                    PagingList = Utilities.GeneratePageList(page, quals.Count, pagingLimit),
                    PagingURL = $"SearchResults?title={title}&page=||_page_||&assessmentMethods={assessmentMethods}&gradingTypes={gradingTypes}&awardingOrganisations={awardingOrganisations}&availability={availability}&qualificationTypes={qualificationTypes}&qualificationLevels={qualificationLevels}&nationalAvailability={nationalAvailability}&minTotalQualificationTime={minTotalQualificationTime}&maxTotalQualificationTime={maxTotalQualificationTime}&minGuidedLearninghours={minGuidedLearninghours}&maxGuidedLearninghours={maxGuidedLearninghours}&sectorSubjectAreas={sectorSubjectAreas}",
                    CurrentPage = quals.CurrentPage
                },
                AppliedFilters = new QualificationAppliedFilterModel
                {
                    AssessmentMethods = assessmentMethods?.GetSubStrings(),
                    GradingTypes = gradingTypes?.GetSubStrings(),
                    AwardingOrganisations = awardingOrganisations?.GetSubStrings(),
                    Availability = availability?.GetSubStrings(),
                    QualificationTypes = qualificationTypes?.GetSubStrings(),
                    QualificationLevels = qualificationLevels?.GetSubStrings(),
                    NationalAvailability = nationalAvailability?.GetSubStrings(),
                    SectorSubjectAreas = sectorSubjectAreas?.GetSubStrings(),
                    MinTotalQualificationTime = minTotalQualificationTime,
                    MaxTotalQualificationTime = maxTotalQualificationTime,
                    MinGuidedLearninghours = minGuidedLearninghours,
                    MaxGuidedLearninghours = maxGuidedLearninghours
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Filters(string title, string[] assessmentMethods, string[] gradingTypes, string[] awardingOrganisations, string[] availability, string[] qualificationTypes, string[] qualificationLevels, string[] nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours, string[] sectorSubjectAreas)
        {
            return RedirectToAction(nameof(SearchResults), new
            {
                title,
                assessmentMethods = string.Join(',', assessmentMethods),
                gradingTypes = string.Join(',', gradingTypes),
                awardingOrganisations = string.Join(',', awardingOrganisations),
                availability = string.Join(',', availability),
                qualificationTypes = string.Join(',', qualificationTypes),
                qualificationLevels = string.Join(',', qualificationLevels),
                nationalAvailability = string.Join(',', nationalAvailability),
                minTotalQualificationTime,
                maxTotalQualificationTime,
                minGuidedLearninghours,
                maxGuidedLearninghours,
                sectorSubjectAreas = string.Join(',', sectorSubjectAreas),
            });
        }

    }
}
