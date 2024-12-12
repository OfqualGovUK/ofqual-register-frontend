using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Ofqual.Common.RegisterFrontend.Controllers;

namespace Ofqual.Common.RegisterFrontend.Tests.Controllers
{
    public class LegacyControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly LegacyController _controller;

        public LegacyControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["RegisterBaseUrl"]).Returns("https://localhost:44320");
            _controller = new LegacyController(_mockConfiguration.Object);
        }

        [Theory]
        [InlineData(null, null, "https://localhost:44320")]
        [InlineData("organisations", null, "https://localhost:44320/find-regulated-organisations")]
        [InlineData("organisations", "testOrg", "https://localhost:44320/organisations/?page=1")]
        [InlineData("qualifications", null, "https://localhost:44320/find-regulated-qualifications/")]
        [InlineData("qualifications", "601/2653/X", "https://localhost:44320/qualifications/6012653X")]
        [InlineData("invalidCategory", "testQuery", "https://localhost:44320")]
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
