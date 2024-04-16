using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models;
using System.Diagnostics;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
