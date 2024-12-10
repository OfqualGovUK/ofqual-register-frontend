using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    /// <summary>
    /// Controller to handle legacy requests and redirect them to appropriate endpoints.
    /// </summary>
    public class LegacyController : ControllerBase
    {
        private readonly string baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyController"/> class.
        /// The constructor retrieves the base URL from the configuration.
        /// the baseurl should point at the correct environment, so for dev local it will be https://localhost:44320/
        /// </summary>
        /// <param name="config">The configuration instance to retrieve settings.</param>
        public LegacyController(IConfiguration config)
        {
            baseUrl = config["RegisterBaseUrl"];
        }

        /// <summary>
        /// Handles GET requests and redirects based on the provided category and query parameters.
        /// </summary>
        /// <param name="category">handle categories "organisations" and "qualifications" anything else go to baseurl </param>
        /// <param name="query"> if no query parameters go to baseurl else handle depending on category type</param>
        /// <returns>A redirect result to the appropriate endpoint based on the parameters.</returns>
        [Route("legacy")]
        [HttpGet]
        public IActionResult Get([FromQuery] string? category, [FromQuery] string? query)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(query))
            {
                if (category.Equals("organisations", StringComparison.OrdinalIgnoreCase))
                {
                    return new RedirectResult($"{baseUrl}/organisations/?page=1&name={query}");
                }
                else if (category.Equals("qualifications", StringComparison.OrdinalIgnoreCase))
                {
                    string q = query.Replace("/", "");
                    return new RedirectResult($"{baseUrl}/qualifications/{q}");
                }
                else
                {
                    return new RedirectResult(baseUrl);
                }
            }
            else
            {
                return new RedirectResult(baseUrl);
            }
        }
    }
}
