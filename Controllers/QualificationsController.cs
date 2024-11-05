using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofqual.Common.RegisterFrontend.Cache;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV;
using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Ofqual.Common.RegisterFrontend.UseCases.Qualifications;
using Refit;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using static Ofqual.Common.RegisterFrontend.Models.Constants;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public partial class QualificationsController : Controller
    {
        private readonly ILogger<QualificationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IRefDataCache _refDataCache;
        private readonly IQualificationsUseCases _qualificationsUseCases;
        private readonly IConfiguration _config;

        [GeneratedRegex("\\b\\d{3}\\/\\d{4}\\/\\w\\b")]
        private static partial Regex QualificationNumRegex();

        [GeneratedRegex("\\b\\d{7}\\w\\b")]
        private static partial Regex QualificationNumNoObliquesRegex();

        public QualificationsController(ILogger<QualificationsController> logger, IRegisterAPIClient registerAPIClient, IConfiguration config, IRefDataCache refDataCache, IQualificationsUseCases qualificationsUseCases)
        {
            _logger = logger;
            _registerAPIClient = registerAPIClient;
            _refDataCache = refDataCache;
            _qualificationsUseCases = qualificationsUseCases;
            _config = config;
        }

        [HttpGet]
        [Route("find-regulated-qualifications/start")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("find-regulated-qualifications")]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("/qualifications")]
        public async Task<IActionResult> SearchResults(string title, string bav = "", int page = 1, string? assessmentMethods = null, string? gradingTypes = null, string[]? awardingOrganisations = null, string? availability = null, string? qualificationTypes = null, string? qualificationLevels = null, string? nationalAvailability = null, int? minTotalQualificationTime = null, int? maxTotalQualificationTime = null, int? minGuidedLearninghours = null, int? maxGuidedLearninghours = null, string? sectorSubjectAreas = null)
        {
            #region Qual Detail
            //redirect to the qualification details if qual number is searched
            if (!string.IsNullOrWhiteSpace(title) && title.Length >= 8)
            {
                if (QualificationNumRegex().IsMatch(title))
                {
                    var qualNumber = title.Split('/');
                    return RedirectToAction(nameof(Qualification), new
                    {
                        number1 = qualNumber[0],
                        number2 = qualNumber[1],
                        number3 = qualNumber[2]
                    });
                }

                if (QualificationNumNoObliquesRegex().IsMatch(title))
                {
                    return RedirectToAction(nameof(Qualification), new
                    {
                        number1 = title[..3],
                        number2 = title[3..7],
                        number3 = title[7]
                    });
                }
            }

            //if quals are searched by any search term, apply the Available to learners filter by default
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(bav))
            {
                availability = AVAILABILITY_AVAILABLE_TO_LEARNERS;
            }
            #endregion

            var pagingURL = $"/qualifications?page=||_page_||";

            var pagedFilters = _qualificationsUseCases.CreatePagedFilters(title, availability, qualificationTypes, qualificationLevels, awardingOrganisations, sectorSubjectAreas, gradingTypes, assessmentMethods, nationalAvailability, minTotalQualificationTime, maxTotalQualificationTime, minGuidedLearninghours, maxGuidedLearninghours);

            //to show the filters applied section on the page
            var filtersApplied = !string.IsNullOrEmpty(pagedFilters);

            if (!string.IsNullOrEmpty(title))
            {
                //title is not part of the filters section
                pagingURL += $"&title={title}";
            }

            pagingURL += pagedFilters;

            #region Filter Values
            var filterValues = new QualificationFilterModel();

            try
            {
                filterValues = await _qualificationsUseCases.GetFilterValues();
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
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
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
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
                    AwardingOrganisations = awardingOrganisations,
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
        [Route("qualifications/filters")]
        //to update the filters on the go from the selected filters on the side
        public IActionResult Filters(string searchTitle, string[] assessmentMethods, string[] gradingTypes, string[] awardingOrganisations, string[] availability, string[] qualificationTypes, string[] qualificationLevels, string[] nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours, string[] sectorSubjectAreas)
        {
            return RedirectToAction(nameof(SearchResults), new
            {
                title = searchTitle,
                assessmentMethods = string.Join(',', assessmentMethods),
                gradingTypes = string.Join(',', gradingTypes),
                awardingOrganisations,
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
        [Route("qualifications/{number1}/{number2}/{number3}")]
        public async Task<IActionResult> Qualification(string number1, string number2, string number3)
        {
            try
            {
                var qual = await _registerAPIClient.GetQualificationAsync(number1, number2, number3);
                return View(qual);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        [HttpGet]
        [Route("qualifications/{number1}")]
        public async Task<IActionResult> QualificationNoObliques(string number1)
        {
            try
            {
                var qual = await _registerAPIClient.GetQualificationAsync(number1);
                return View("Qualification", qual);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        [HttpGet]
        [Route("qualifications/download-CSV")]
        public async Task<IActionResult> DownloadCSV(string? title, string? availability, string? qualificationTypes, string? qualificationLevels, string[]? awardingOrganisations, string? sectorSubjectAreas, string? gradingTypes, string? assessmentMethods, string? nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours)
        {
            var titleName = string.IsNullOrEmpty(title) ? "" : "_" + title;

            string fileName = $"Qualifications{titleName}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];
            try
            {

                APIResponseList<QualificationCSV> quals;

                try
                {
                    quals = await _registerAPIClient.GetFullQualificationsDataSetAsync(title, page: 1, limit: 0, assessmentMethods: assessmentMethods, gradingTypes: gradingTypes, awardingOrganisations: awardingOrganisations, availability: availability, qualificationTypes: qualificationTypes, qualificationLevels: qualificationLevels, nationalAvailability: nationalAvailability, sectorSubjectAreas: sectorSubjectAreas, minTotalQualificationTime: minTotalQualificationTime, maxTotalQualificationTime: maxTotalQualificationTime, minGuidedLearninghours: minGuidedLearninghours, maxGuidedLearninghours: maxGuidedLearninghours);
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                }

                using var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    var options = new TypeConverterOptions { Formats = ["yyyy/MM/dd HH:mm:ss"] };
                    csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                    csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

                    csvWriter.WriteRecords(quals.Results);
                }

                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        #region Compare

        [HttpGet]
        [Route("qualifications/compare-qualifications")]
        ///selectedQuals if JS is enabled - will retain all quals selected across pages
        ///QualificationNumber if JS is disabled - will only retain the quals selected for this page
        public IActionResult CompareQualifications(string? selectedQuals, string[] qualificationNumbers)
        {
            var compareArr = selectedQuals != null ? selectedQuals.Split(',') : qualificationNumbers;

            // less than 2 quals are selected (for no JS where user can select one qual and hit compare / download CSV)
            // go back to the search results page and show an error
            if (compareArr == null || compareArr.Length < 2)
            {
                TempData["CompareError"] = true;
                return Redirect(Request.Headers.Referer);
            }

            if (compareArr.Length >= 2)
            {
                //first two will be compared
                var selected = $"{compareArr[0]},{compareArr[1]}";

                //rest will be unselected
                string unselected = "";

                //start from the 3rd item (index 2)
                for (int i = 2; i < compareArr.Length; i++)
                {
                    unselected += compareArr[i] + (i != (compareArr.Length - 1) ? "," : "");
                }

                return RedirectToAction(nameof(Compare), new { selected, unselected });
            }

            return RedirectToAction("Search");
        }

        [HttpGet]
        [Route("qualifications/compare")]
        public async Task<IActionResult> Compare(string selected, string? unselected)
        {
            if (string.IsNullOrEmpty(selected))
            {
                return RedirectToAction(nameof(Search));
            }

            var selectedQuals = selected.Split(",");
            var model = new CompareQualsModel();

            try
            {
                var left = await _registerAPIClient.GetQualificationAsync(selectedQuals[0]);
                var right = await _registerAPIClient.GetQualificationAsync(selectedQuals[1]);

                var differing = _qualificationsUseCases.CompareQuals(left, right);

                model = new CompareQualsModel
                {
                    Left = left,
                    Right = right,
                    SelectedQuals = selected,
                    UnselectedQuals = unselected,
                    Differing = differing,
                };
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }

            return View(model);
        }

        [HttpGet]
        [Route("qualifications/compare/change")]
        public async Task<IActionResult> ChangeCompare(string current, string selected, string unselected)
        {
            if (string.IsNullOrEmpty(current))
            {
                return RedirectToAction(nameof(Search));
            }

            var quals = selected.Split(",");

            if (!string.IsNullOrEmpty(unselected))
            {
                quals = quals.Concat(unselected.Split(",")).ToArray();
            }

            var qualDetails = new Dictionary<string, string>();

            var unselectedQuals = !string.IsNullOrEmpty(unselected) ? unselected.Split(",") : [];

            if (unselectedQuals.Length > 0)
            {
                foreach (var item in unselectedQuals)
                {
                    try
                    {
                        var qual = await _registerAPIClient.GetQualificationAsync(item);
                        qualDetails.Add(item, qual.Title);
                    }
                    catch (ApiException ex)
                    {
                        return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                    }
                }
            }

            return View(qualDetails);
        }

        [HttpPost]
        [Route("qualifications/compare/change")]
        public IActionResult SubmitChangeCompare(string changeQualification, string current, string selected, string unselected)
        {
            if (string.IsNullOrEmpty(unselected))
            {
                return RedirectToAction(nameof(Compare), new { selected, unselected = "" });
            }

            if (string.IsNullOrEmpty(selected) || string.IsNullOrEmpty(current))
            {
                return RedirectToAction(nameof(Search));
            }

            if (string.IsNullOrEmpty(changeQualification))
            {
                return RedirectToAction(nameof(Compare), new { selected, unselected });
            }

            selected = selected.Replace(current, changeQualification);

            unselected = unselected.Replace(changeQualification, current);


            return RedirectToAction(nameof(Compare), new { selected, unselected });
        }
        #endregion

    }
}
