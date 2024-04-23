using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class OrganisationsController : Controller
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IConfiguration _config;


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
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string name, int page = 1)
        {
            //check for qualification number regex
            string? numberRN = null;
            string pattern = @"^\d+$";
            int pagingLimit = _config.GetValue<int>("OrganisationsPagingLimit");

            var orgs = await _registerAPIClient.GetOrganisationsListAsync(name, page, pagingLimit);

            var model = new SearchResultViewModel<OrganisationListViewModel> { 
                List = orgs, 
                Name = name, 
                PagingURL = $"SearchResults?name={name}&page=||_page_||" ,
                PagingList = Utilities.GeneratePageList(page, orgs.Count, pagingLimit)
            };

            return View(model);
        }

    }
}
