using Microsoft.Playwright;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages
{
    public class OrganisationDetailsPage : BasePage
    {
        private readonly ILocator _findAllQualificationsLink;

        public OrganisationDetailsPage(IPage page) : base(page)
        {
            _findAllQualificationsLink = page.Locator("a.govuk-button");
        }

        public async Task clickFindAllQualificationsLink()
        {
            await _findAllQualificationsLink.ClickAsync();

        }

    }
}