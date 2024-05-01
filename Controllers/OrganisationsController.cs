using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public partial class OrganisationsController : Controller
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IConfiguration _config;

        [GeneratedRegex(@"(RN)?(rn)?\d+")]
        private static partial Regex OrgNumberRegex();

        public OrganisationsController(ILogger<OrganisationsController> logger, IRegisterAPIClient registerAPIClient, IConfiguration config)
        {
            _logger = logger;
            _registerAPIClient = registerAPIClient;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Organisations/Search")]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("Organisations/SearchResults")]
        public async Task<IActionResult> SearchResults(string name, int page = 1)
        {
            //if search term is an org recognition number, show the org details page
            if (!string.IsNullOrEmpty(name) && OrgNumberRegex().IsMatch(name))
            {
                if (!name.Substring(0, 2).ToLower().Equals("rn"))
                {
                    name = $"RN{name}";
                }

                return RedirectToAction("Organisation", new { number = name.ToUpper() });
            }

            int pagingLimit = _config.GetValue<int>("OrganisationsPagingLimit");

            APIResponseList<OrganisationListViewModel> orgs;

            try
            {
                orgs = await _registerAPIClient.GetOrganisationsListAsync(name, page, pagingLimit);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }

            var model = new SearchResultViewModel<OrganisationListViewModel>
            {
                List = orgs,
                Title = name,
                Paging = new PagingModel
                {
                    PagingList = Utilities.GeneratePageList(page, orgs.Count, pagingLimit),
                    PagingURL = $"SearchResults?name={name}&page=||_page_||",
                    CurrentPage = orgs.CurrentPage
                }
            };

            return View(model);
        }


        [HttpGet]
        [Route("Organisations/{number}")]
        public async Task<IActionResult> Organisation(string number)
        {
            try
            {
                var org = await _registerAPIClient.GetOrganisationAsync(number);
                return View(org);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Organisations/DownloadCSV")]
        public async Task<IActionResult> DownloadCSV(string? name)
        {
            APIResponseList<Organisation> orgs;

            name = string.IsNullOrEmpty(name) ? "" : "_" + name;

            string fileName = $"Organisations{name}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];

            try
            {
                orgs = await _registerAPIClient.GetOrganisationsDetailListAsync(name, 1, 500);

                using var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(orgs.Results);
                }

                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return File(fileBytes, "text/csv", fileName);
            }
        }

    }
}
