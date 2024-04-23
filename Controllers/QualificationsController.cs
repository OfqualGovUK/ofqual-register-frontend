using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using System.Diagnostics;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class QualificationsController : Controller
    {
        private readonly ILogger<QualificationsController> _logger;
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IConfiguration _config;


        public QualificationsController(ILogger<QualificationsController> logger, IRegisterAPIClient registerAPIClient, IConfiguration config)
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
        public IActionResult Search() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string title, int page = 1)
        {
            //check for qualification number regex
            
            int pagingLimit = _config.GetValue<int>("QualificationsPagingLimit");

            var quals = await _registerAPIClient.GetQualificationsListAsync(title, page, pagingLimit);

            var model = new SearchResultViewModel<QualificationListViewModel>
            {
                List = quals,
                Title = title,
                PagingURL = $"SearchResults?title={title}&page=||_page_||",
                PagingList = Utilities.GeneratePageList(page, quals.Count, pagingLimit)
            };

            return View(model);
        }
    }
}
