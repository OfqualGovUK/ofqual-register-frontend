using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("500")]
        public IActionResult AppError()
        {
            return View("404");
        }

        [Route("404")]
        public IActionResult NotFoundError()
        {
            return View("404");
        }
    }
}
