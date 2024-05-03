using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        [HttpGet]
        [Route("find-regulated-qualifications")]
        //[Route("Qualifications/Search")]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("qualifications")]
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
            else if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(bav))
            {
                availability = "Available to learners";
            }
            #endregion

            #region Paging Url
            var pagingURL = $"qualifications?page=||_page_||";

            //to show the filters applied section on the page
            var filtersApplied = false;

            if (!string.IsNullOrEmpty(title))
            {
                //title is not part of the filters section
                pagingURL += $"&title={title}";
            }

            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(availability), availability);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(qualificationTypes), qualificationTypes);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(qualificationLevels), qualificationLevels);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(awardingOrganisations), awardingOrganisations);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(sectorSubjectAreas), sectorSubjectAreas);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(gradingTypes), gradingTypes);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(assessmentMethods), assessmentMethods);
            AppendFilterToPaging(ref pagingURL, ref filtersApplied, nameof(nationalAvailability), nationalAvailability);

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
        //to update the filters on the go from the selected filters on the side
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
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
        }

        [HttpGet]
        [Route("qualifications/download-CSV")]
        public async Task<IActionResult> DownloadCSV(string title, string? selectedQuals, string[] QualificationNumbers)
        {
            title = string.IsNullOrEmpty(title) ? "" : "_" + title;

            string fileName = $"Qualifications{title}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];

            try
            {
                var quals = selectedQuals != null ? selectedQuals.Split(',') : QualificationNumbers;

                var qualsDetails = new List<Qualification>();

                foreach (var item in quals)
                {
                    qualsDetails.Add(await _registerAPIClient.GetQualificationAsync(item));
                }

                using var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(qualsDetails);
                }

                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return File(fileBytes, "text/csv", fileName);
            }
        }

        #region Compare

        [HttpGet]
        [Route("qualifications/compare-qualifications")]
        ///selectedQuals if JS is enabled - will retain all quals selected across pages
        ///QualificationNumber is JS is disabled - will only retain the quals selected for this page
        public IActionResult CompareQualifications(string title, string? selectedQuals, string[] QualificationNumbers)
        {
            if (Request.Query["CSV"].Count != 0)
            {
                return RedirectToAction("download-CSV", new { title, selectedQuals, QualificationNumbers });
            }

            var compareArr = selectedQuals != null ? selectedQuals.Split(',') : QualificationNumbers;

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

                return RedirectToAction("compare", new { selected, unselected });
            }

            return RedirectToAction("Search");
        }

        [HttpGet]
        [Route("qualifications/compare")]
        public async Task<IActionResult> Compare(string selected, string? unselected)
        {
            if (string.IsNullOrEmpty(selected))
            {
                return RedirectToAction("Search");
            }

            var selectedQuals = selected.Split(",");

            var left = await _registerAPIClient.GetQualificationAsync(selectedQuals[0]);
            var right = await _registerAPIClient.GetQualificationAsync(selectedQuals[1]);

            var differing = new Dictionary<string, string[]>();

            DiffValues(left.Type, right.Type, "Qualification type", ref differing);
            DiffValues(left.Level, right.Level, "Qualification level", ref differing);
            DiffValues(left.QualificationNumber, right.QualificationNumber, "Qualification number", ref differing);
            DiffValues(left.GLH.ToString(), right.GLH.ToString(), "Guided learning hours", ref differing);
            DiffValues(left.TQT.ToString(), right.TQT.ToString(), "Total qualification time", ref differing);

            var leftAssessmentMethods = left.AssessmentMethods == null ? null : string.Join("<br />", left.AssessmentMethods);
            var rightAssessmentMethods = right.AssessmentMethods == null ? null : string.Join("<br />", right.AssessmentMethods);

            DiffValues(leftAssessmentMethods, rightAssessmentMethods, "Assessment methods", ref differing);

            DiffValues(left.LinkToSpecification, right.LinkToSpecification, "Specification", ref differing);
            DiffValues(left.SSA, right.SSA, "Sector subject area", ref differing);
            DiffValues(left.GradingScale, right.GradingScale, "Grading scale", ref differing);
            DiffValues($"{left.ApprenticeshipStandardTitle} ({left.ApprenticeshipStandardReferenceNumber})", $"{right.ApprenticeshipStandardTitle} ({right.ApprenticeshipStandardReferenceNumber})", "End-point assessment standard", ref differing);
            DiffValues(left.OrganisationName, right.OrganisationName, "Awarding Organisation", ref differing);
            DiffValues(left.GradingType, right.GradingType, "Grading type", ref differing);
            DiffValues(left.Specialism, right.Specialism, "Specialisms", ref differing);
            DiffValues(left.QualificationNumberNoObliques, right.QualificationNumberNoObliques, "Funding in England", ref differing);
            DiffValues(left.Status, right.Status, "Status", ref differing);

            //national availability
            var leftNationalAvailability = "";
            if (left.OfferedInEngland ?? false)
            {
                leftNationalAvailability += "England, ";
            }
            if (left.OfferedInNorthernIreland ?? false)
            {
                leftNationalAvailability += "Northern Ireland, ";
            }
            if (left.OfferedInternationally ?? false)
            {
                leftNationalAvailability += "International";
            }

            var rightNationalAvailability = "";
            if (right.OfferedInEngland ?? false)
            {
                rightNationalAvailability += "England, ";
            }
            if (right.OfferedInNorthernIreland ?? false)
            {
                rightNationalAvailability += "Northern Ireland, ";
            }
            if (right.OfferedInternationally ?? false)
            {
                rightNationalAvailability += "International";
            }

            DiffValues(leftNationalAvailability, rightNationalAvailability, "National availability", ref differing);
            DiffValues(left.OfferedInNorthernIreland.ToString(), right.OfferedInNorthernIreland.ToString(), "Available in Northern Ireland", ref differing);

            DiffValues(left.RegulationStartDate.ToString("dd MMMM yyyy"), right.RegulationStartDate.ToString("dd MMMM yyyy"), "Regulation start date", ref differing);
            DiffValues(left.OperationalStartDate.ToString("dd MMMM yyyy"), right.OperationalStartDate.ToString("dd MMMM yyyy"), "Operational start date", ref differing);
            DiffValues(left.OperationalEndDate?.ToString("dd MMMM yyyy"), right.OperationalEndDate?.ToString("dd MMMM yyyy"), "Operational end date", ref differing);
            DiffValues(left.CertificationEndDate?.ToString("dd MMMM yyyy"), right.CertificationEndDate?.ToString("dd MMMM yyyy"), "Certification end date", ref differing);
            DiffValues(left.EQFLevel, right.EQFLevel, "European qualification level", ref differing);
            DiffValues(left.Pathways, right.Pathways, "Optional Routes", ref differing);

            var model = new CompareQualsModel
            {
                Left = left,
                Right = right,
                SelectedQuals = selected,
                UnselectedQuals = unselected,
                Differing = differing,
            };

            return View(model);
        }

        [HttpGet]
        [Route("qualifications/compare/change")]
        public async Task<IActionResult> ChangeCompare(string current, string selected, string unselected)
        {
            if (string.IsNullOrEmpty(current))
            {
                return RedirectToAction("Search");
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
                    var qual = await _registerAPIClient.GetQualificationAsync(item);
                    qualDetails.Add(item, qual.Title);
                }
            }

            return View(qualDetails);
        }

        [HttpPost]
        [Route("Qualifications/Compare/Change")]
        public IActionResult SubmitChangeCompare(string changeQualification, string current, string selected, string unselected)
        {
            if (string.IsNullOrEmpty(unselected))
            {
                return RedirectToAction("Compare", new { selected, unselected = "" });
            }

            if (string.IsNullOrEmpty(selected) || string.IsNullOrEmpty(current) || string.IsNullOrEmpty(changeQualification))
            {
                return RedirectToAction("Search");
            }

            selected = selected.Replace(current, changeQualification);

            unselected = unselected.Replace(changeQualification, current);


            return RedirectToAction("Compare", new { selected, unselected });
        }

        #endregion

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
        void AppendFilterToPaging(ref string url, ref bool filtersApplied, string paramName, string? param)
        {
            if (param != null && param.GetSubStrings() != null)
            {
                url += $"&{paramName}={param}";
                filtersApplied = true;
            }
        }

        // Helper method to compare and add differing fields in the quals compare
        private static void DiffValues(string? left, string? right, string fieldName, ref Dictionary<string, string[]> differing)
        {
            left = string.IsNullOrEmpty(left) ? null : left;
            right = string.IsNullOrEmpty(right) ? null : right;

            if (left != right)
            {
                differing.Add(fieldName, [left ?? "-", right ?? "-"]);
            }
        }
        #endregion

    }
}
