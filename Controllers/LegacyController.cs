using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    /// <summary>
    /// Controller to handle legacy requests and redirect them to appropriate endpoints.
    /// </summary>
    public class LegacyController : ControllerBase
    {
        private readonly string _baseUrl;
        private readonly string _defaultUrl;


        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyController"/> class.
        /// The constructor retrieves the base URL from the configuration.
        /// The base URL should point to the correct environment, so for dev local it will be https://localhost:44320/
        /// </summary>
        /// <param name="config">The configuration instance to retrieve settings.</param>
        public LegacyController(IConfiguration config)
        {
            _baseUrl = config["RegisterBaseUrl"]; // in production this should be https://find-a-qualification.services.ofqual.gov.uk/
            _defaultUrl = config["DefaultUrl"];   // in production this should be https://www.gov.uk/find-a-regulated-qualification
        }

        /// <summary>
        /// Handles GET requests and redirects based on the provided category and query parameters.
        /// </summary>
        /// <param name="category">Handles categories "organisations" and "qualifications". Anything else redirects to the base URL.</param>
        /// <param name="query">If no query parameter is provided, redirects to the base URL. Otherwise, handles depending on the category type.</param>
        /// <returns>A redirect result to the appropriate endpoint based on the parameters.</returns>
        [Route("legacy")]
        [HttpGet]
        public IActionResult Get([FromQuery] string? category, [FromQuery] string? query)
        {
            // If the category is null or empty, redirect to the base URL
            if (string.IsNullOrEmpty(category))
            {
                return new RedirectResult(_defaultUrl);
            }

            // Determine the redirect URL based on the category and query parameters
            string redirectUrl = category.ToLower() switch
            {
                "organisations" => string.IsNullOrEmpty(query) ? $"{_baseUrl}/find-regulated-organisations" : $"{_baseUrl}/organisations/?page=1&name={query}",
                "qualifications" => string.IsNullOrEmpty(query) ? $"{_baseUrl}/find-regulated-qualifications/" : $"{_baseUrl}/qualifications/{query.Replace("/", "")}",
                _ => _defaultUrl
            };

            // Redirect to the determined URL
            return new RedirectResult(redirectUrl);
        }
    }
}
