using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class OrganisationsController : Controller
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;


        public OrganisationsController(ILogger<OrganisationsController> logger, IRegisterAPIClient registerAPIClient)
        {
            _logger = logger;
            _registerAPIClient = registerAPIClient;
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
        public async Task<IActionResult> SearchResults(string name, int page = 1, int limit = 10)
        {
            //check for qualification number regex
            string? numberRN = null;
            string pattern = @"^\d+$";

            if (name != null)
            {
                Match m = Regex.Match(name, pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    numberRN = $"RN{name}";
                }
                else if (name!.Substring(0, 2).Equals("rn", StringComparison.InvariantCultureIgnoreCase))
                {
                    numberRN = name;
                }
            }

            //if (numberRN != null)
            //{
            //    return RedirectToAction("Organisations", new { id = numberRN });
            //}

            var orgs = await _registerAPIClient.GetOrganisationListAsync(name, page, limit);
            var model = new OrganisationSearchResultViewModel { List = orgs, Name = name };

            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> Index(string number)
        //{
        //    var org = await _registerAPIClient.GetOrganisationAsync(number);

        //    return View("Organisation", org);
        //}
    }
}
