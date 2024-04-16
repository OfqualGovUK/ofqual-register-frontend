using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models;
using System.Diagnostics;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class QualificationsController : Controller
    {
        private readonly ILogger<QualificationsController> _logger;

        public QualificationsController(ILogger<QualificationsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search() {
            return View();
        }


        [HttpPost]
        public IActionResult Search(string title)
        {
            //check for qualification number regex



            return View("SearchResults", title);
        }
    }
}
