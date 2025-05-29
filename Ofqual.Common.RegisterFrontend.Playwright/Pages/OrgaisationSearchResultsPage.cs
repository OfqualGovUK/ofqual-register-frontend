using Microsoft.Playwright;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages
{
    public class OrganisationSearchResultsPage : BasePage
    {
        private readonly ILocator _searchInputBox;
        private readonly ILocator _organisationLink;
        private readonly ILocator _organisationItems;
        private readonly ILocator _paginationSection;

        public OrganisationSearchResultsPage(IPage page) : base(page)
        {
            _searchInputBox = page.Locator("#name");
            _organisationLink = page.Locator("ul li p a.govuk-link");
            _organisationItems = page.Locator("li.results-list-item");
            _paginationSection = page.Locator(".govuk-pagination");
        }

        public async Task VerifySearchCriteria(String expectedSearchTerm)
        {
            await Expect(_searchInputBox).ToHaveValueAsync(expectedSearchTerm);

        }

        public async Task ClickOrganisationLink()
        {
            await _organisationLink.ClickAsync();
        }

        public async Task VerifyOrgCountAndPagination(int expectedOrgsCount)
        {
            int totalOrgsPerPage = await _organisationItems.CountAsync();
            bool isPaginationVisible = await _paginationSection.IsVisibleAsync();

            Assert.That(totalOrgsPerPage, Is.EqualTo(expectedOrgsCount));
            Assert.True(isPaginationVisible);
        }

    }
}