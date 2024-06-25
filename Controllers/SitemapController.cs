using Microsoft.AspNetCore.Mvc;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class SitemapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("sitemap/organisations")]
        public IActionResult Organisations()
        {
            return View();
        }

        [HttpGet]
        [Route("sitemap/qualifications")]
        public IActionResult Qualifications()
        {
            return View();
        }
    }
}
