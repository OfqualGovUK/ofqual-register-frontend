using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public partial class QualificationsController : Controller
    {
        private readonly ILogger<QualificationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IRefDataAPIClient _refDataAPIClient;
        private readonly IConfiguration _config;

        [GeneratedRegex("\\b\\d{3}\\/\\d{4}\\/\\w\\b")]
        private static partial Regex QualificationNumRegex();

        [GeneratedRegex("\\b\\d{7}\\w\\b")]
        private static partial Regex QualificationNumNoObliquesRegex();

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
        public async Task<IActionResult> SearchResults(string title, string bav = "", int page = 1, string? assessmentMethods = null, string? gradingTypes = null, string? awardingOrganisations = null, string? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, int? minTotalQualificationTime = null, int? maxTotalQualificationTime = null, int? minGuidedLearninghours = null, int? maxGuidedLearninghours = null, string? sectorSubjectAreas = null)
        {
            #region Qual Detail
            //redirect to the qualification details if qual number is searched
            if (!string.IsNullOrWhiteSpace(title) && title.Length >= 8)
            {
                if (QualificationNumRegex().IsMatch(title))
                {
                    var qualNumber = title.Split('/');
                    return RedirectToAction("Qualification", new
                    {
                        number1 = qualNumber[0],
                        number2 = qualNumber[1],
                        number3 = qualNumber[2]
                    });
                }

                if (QualificationNumNoObliquesRegex().IsMatch(title))
                {
                    return RedirectToAction("Qualification", new
                    {
                        number1 = title[..3],
                        number2 = title[3..7],
                        number3 = title[7]
                    });
                }
            }
            else if (!string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(bav))
            {
                availability = "Available to learners";
            }
            #endregion

            #region Paging Url
            var pagingURL = $"SearchResults?page=||_page_||";

            //to show the filters applied section on the page
            var filtersApplied = false;

            if (!string.IsNullOrEmpty(title))
            {
                //title is not part of the filters section
                pagingURL += $"&title={title}";
            }

            AppendFilterToPaging(ref pagingURL, ref filtersApplied, assessmentMethods);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, gradingTypes);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, awardingOrganisations);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, availability);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, qualificationTypes);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nationalAvailability);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, sectorSubjectAreas);

            if (minTotalQualificationTime != null)
            {
                filtersApplied = true;
                pagingURL += $"&minTotalQualificationTime={minTotalQualificationTime}";
            }

            if (maxTotalQualificationTime != null)
            {
                filtersApplied = true;
                pagingURL += $"&maxTotalQualificationTime={maxTotalQualificationTime}";
            }

            if (minGuidedLearninghours != null)
            {
                filtersApplied = true;
                pagingURL += $"&minGuidedLearninghours={minGuidedLearninghours}";
            }

            if (maxGuidedLearninghours != null)
            {
                filtersApplied = true;
                pagingURL += $"&maxGuidedLearninghours={maxGuidedLearninghours}";
            }

            #endregion

            #region Filter Values
            var filterValues = new QualificationFilterModel();

            try
            {
                filterValues = await GetFilterValues();
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
            #endregion

            #region Get Qual Data
            int pagingLimit = _config.GetValue<int>("QualificationsPagingLimit");

            APIResponseList<QualificationListViewModel> quals;

            try
            {
                quals = await _registerAPIClient.GetQualificationsListAsync(title, page, pagingLimit, assessmentMethods: assessmentMethods, gradingTypes: gradingTypes, awardingOrganisations: awardingOrganisations, availability: availability, qualificationTypes: qualificationTypes, qualificationLevels: qualificationLevels, nationalAvailability: nationalAvailability, sectorSubjectAreas: sectorSubjectAreas, minTotalQualificationTime: minTotalQualificationTime, maxTotalQualificationTime: maxTotalQualificationTime, minGuidedLearninghours: minGuidedLearninghours, maxGuidedLearninghours: maxGuidedLearninghours);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }

            #endregion

            var model = new SearchResultViewModel<QualificationListViewModel>
            {
                List = quals,
                Title = title,
                Filters = filterValues,
                Paging = new PagingModel
                {
                    PagingList = Utilities.GeneratePageList(page, quals.Count, pagingLimit),
                    PagingURL = pagingURL,
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
                    MaxGuidedLearninghours = maxGuidedLearninghours,
                    FiltersApplied = filtersApplied
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

        [HttpGet]
        [Route("Qualifications/{number1}/{number2}/{number3}")]
        public async Task<IActionResult> Qualification(string number1, string number2, string number3)
        {
            try
            {
                var qual = await _registerAPIClient.GetQualificationAsync(number1, number2, number3);
                return View(qual);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
        }


        #region Helper methods

        //helper method to get the filter values from the different APIs
        private async Task<QualificationFilterModel> GetFilterValues()
        {

            List<SSA> ssa = await _refDataAPIClient.GetSSAAsync();

            List<Level> levels = await _refDataAPIClient.GetLevelsAsync();

            List<AssessmentMethod> assessMethods = await _refDataAPIClient.GetAssessmentMethodsAsync();

            List<QualificationType> types = await _refDataAPIClient.GetQualificationTypesAsync();

            APIResponseList<OrganisationListViewModel> organisations = await _registerAPIClient.GetOrganisationsListAsync(null, 1, 500);


            return new QualificationFilterModel
            {
                AssessmentMethods = assessMethods.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                QualificationLevels = levels.OrderBy(e => e.LevelDescription).Select(e => e.LevelDescription).ToHashSet(),
                QualificationTypes = types.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                SSA = ssa.OrderBy(e => e.SsaDescription2).Select(e => e.SsaDescription2).ToHashSet(),
                Organisations = organisations.Results!.OrderBy(e => e.Name).Select(e => e.Name).ToHashSet()
            };

        }

        // Helper method to append filter to the paging URL if the filter value is not null or empty
        void AppendFilterToPaging(ref string url, ref bool filtersApplied, string? param)
        {
            if (param != null && param.GetSubStrings() != null)
            {
                url += $"&{nameof(param)}={param}";
                filtersApplied = true;
            }
        }
        #endregion

    }
}
