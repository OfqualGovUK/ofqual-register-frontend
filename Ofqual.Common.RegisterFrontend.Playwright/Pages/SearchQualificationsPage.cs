using Microsoft.Playwright;


namespace Ofqual.Common.RegisterFrontend.Playwright.Pages
{
    public class SearchQualificationsPage : BasePage
    {
        private readonly ILocator _searchQualificationField;
        private readonly ILocator _searchQualificationsButton;
        private readonly ILocator _showAllQualificationsButton;

        public SearchQualificationsPage(IPage page) : base(page)
        {
            _searchQualificationField = page.Locator("#title");
            _searchQualificationsButton = page.GetByRole(AriaRole.Button, new() { Name = "Search qualification" });
            _showAllQualificationsButton = page.GetByRole(AriaRole.Button, new() { Name = "Show all qualifications" });
        }

        public async Task EnterQualificationSearchTerm(String qualificationSearchText)
        {
            await _searchQualificationField.FillAsync(qualificationSearchText);
        }

        public async Task ClickSearchQualifications()
        {
            await _searchQualificationsButton.ClickAsync();
        }

        public async Task ClickShowAllQualifications()
        {
            await _showAllQualificationsButton.ClickAsync();
        }

    }
}