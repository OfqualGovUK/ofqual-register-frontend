using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.RefDataModels;
using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public partial class OrganisationsController : Controller
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IConfiguration _config;

        [GeneratedRegex(@"(RN)?(rn)?\d+$")]
        private static partial Regex OrgNumberRegex();

        public OrganisationsController(ILogger<OrganisationsController> logger, IRegisterAPIClient registerAPIClient, IConfiguration config)
        {
            _logger = logger;
            _registerAPIClient = registerAPIClient;
            _config = config;
        }

        [HttpGet]
        [Route("find-regulated-organisations/start")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("find-regulated-organisations")]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("/organisations")]
        public async Task<IActionResult> SearchResults(string name, int page = 1)
        {
            //if search term is an org recognition number, show the org details page
            if (!string.IsNullOrEmpty(name) && OrgNumberRegex().IsMatch(name))
            {
                if (!name.Substring(0, 2).ToLower().Equals("rn"))
                {
                    name = $"RN{name}";
                }

                return RedirectToAction(nameof(Organisation), new
                {
                    number = name.ToUpper()
                });
            }

            int pagingLimit = _config.GetValue<int>("OrganisationsPagingLimit");
            var pagingURL = $"/organisations?page=||_page_||";

            if (!string.IsNullOrEmpty(name))
            {
                //title is not part of the filters section
                pagingURL += $"&name={name}";
            }

            APIResponseList<OrganisationListViewModel> orgs;

            try
            {
                orgs = await _registerAPIClient.GetOrganisationsListAsync(name, page, pagingLimit);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }

            var model = new SearchResultViewModel<OrganisationListViewModel>
            {
                List = orgs,
                Title = name,
                Paging = new PagingModel
                {
                    PagingList = Utilities.GeneratePageList(page, orgs.Count, pagingLimit),
                    PagingURL = pagingURL,
                    CurrentPage = orgs.CurrentPage
                }
            };

            return View(model);
        }


        [HttpGet]
        [Route("organisations/{number}")]
        public async Task<IActionResult> Organisation(string number)
        {
            try
            {
                var org = await _registerAPIClient.GetOrganisationAsync(number);

                if (org != null)
                {
                    org.RecognitionScope = await _registerAPIClient.GetOrganisationsScopes(number);
                }

                return View(org);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        [HttpGet]
        [Route("Organisations/download-CSV")]
        public async Task<IActionResult> DownloadCSV(string? name)
        {
            var nm = string.IsNullOrEmpty(name) ? "" : "_" + name;

            string fileName = $"Organisations{nm}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];
            try
            {
                APIResponseList<Organisation> orgs;
                orgs = await _registerAPIClient.GetOrganisationsDetailListAsync(name);

                using var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    var options = new TypeConverterOptions { Formats = ["yyyy/MM/dd HH:mm:ss"] };
                    csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                    csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

                    csvWriter.WriteRecords(orgs.Results);
                }

                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        [HttpGet]
        [Route("Organisations/download-scopes-CSV/{recognitionNumber}")]
        public async Task<IActionResult> DownloadScopesCSV(string recognitionNumber)
        {
            string fileName = $"{recognitionNumber}_Scope_of_recognition_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];
            try
            {
                var org = await _registerAPIClient.GetOrganisationAsync(recognitionNumber);
                var scopes = await _registerAPIClient.GetOrganisationsScopes(recognitionNumber);

                fileName = $"{org.Name}_{recognitionNumber}_Scope_of_recognition_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";

                using var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(CreateScopesCSV(scopes, recognitionNumber, org.Name));
                }

                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }
        }

        private List<RecognitionScopeCSVModel> CreateScopesCSV(RecognitionScope scope, string recognitionNumber, string orgName)
        {
            var list = new List<RecognitionScopeCSVModel>();

            foreach (var type in scope.Inclusions)
            {
                foreach (var level in type.Levels)
                {
                    foreach (var qual in level.Recognitions)
                    {
                        list.Add(new RecognitionScopeCSVModel
                        {
                            RecognitionNumber = recognitionNumber,
                            OrganisationName = orgName,
                            Level = level.Level,
                            Regulated = true,
                            Title = qual,
                            Type = type.Type
                        });
                    }
                }
            }

            foreach (var unregulated in scope.Exclusions)
            {
                list.Add(new RecognitionScopeCSVModel
                {
                    RecognitionNumber = recognitionNumber,
                    OrganisationName = orgName,
                    Level = string.Empty,
                    Regulated = false,
                    Title = unregulated,
                    Type = string.Empty
                });
            }

            return list;
        }

    }
}
