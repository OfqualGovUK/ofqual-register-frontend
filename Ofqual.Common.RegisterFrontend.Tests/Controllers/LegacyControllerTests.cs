using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Ofqual.Common.RegisterFrontend.Controllers;
using static System.Net.WebRequestMethods;

namespace Ofqual.Common.RegisterFrontend.Tests.Controllers
{
    public class LegacyControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly LegacyController _controller;
        const string registerBaseUrl = "https://find-a-qualification.services.ofqual.gov.uk";
        const string defaultUrl = "https://www.gov.uk/find-a-regulated-qualification";

        public LegacyControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["RegisterBaseUrl"]).Returns(registerBaseUrl);
            _mockConfiguration.Setup(config => config["DefaultUrl"]).Returns(defaultUrl);
            _controller = new LegacyController(_mockConfiguration.Object);
        }

        [Theory]
        [InlineData(null, null, defaultUrl)]
        [InlineData("organisations", null, registerBaseUrl + "/find-regulated-organisations")]
        [InlineData("organisations", "testOrg", registerBaseUrl + "/organisations/?page=1&name=testOrg")]
        [InlineData("qualifications", null, registerBaseUrl + "/find-regulated-qualifications/")]
        [InlineData("qualifications", "601/2653/X", registerBaseUrl + "/qualifications/6012653X")]
        [InlineData("invalidCategory", "testQuery", defaultUrl)]
        [Trait("Category", "Unit")]
        public void Get_ReturnsExpectedRedirectUrl(string category, string query, string expectedUrl)
        {
            // Act
            var result = _controller.Get(category, query) as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUrl, result.Url);
        }
    }
}
