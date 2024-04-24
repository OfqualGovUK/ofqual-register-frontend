using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;

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
        [Route("Organisations/Search")]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Route("Organisations/SearchResults")]
        public async Task<IActionResult> SearchResults(string name, int page = 1)
        {
            int pagingLimit = _config.GetValue<int>("OrganisationsPagingLimit");

            var orgs = await _registerAPIClient.GetOrganisationsListAsync(name, page, pagingLimit);

            var model = new SearchResultViewModel<OrganisationListViewModel> { 
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
            var org = await _registerAPIClient.GetOrganisationAsync(number);

            return View(org);
        }

    }
}
