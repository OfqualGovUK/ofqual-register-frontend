using Microsoft.Playwright;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages
{
    public class CompareQualificationsPage : BasePage
    {
        private readonly ILocator _subHeadings;
        private readonly ILocator _differingInfoCards;
        private readonly ILocator _changeQualificationButtons;

        public CompareQualificationsPage(IPage page) : base(page)
        {
            _subHeadings = page.Locator("#main-content h2");
            _differingInfoCards = page.Locator("[id*='differences']");
            _changeQualificationButtons = page.Locator("a.govuk-button");
        }

        public async Task VerifyComparisionDetails()
        {
            var totalQualHeadings = await _subHeadings.CountAsync();
            var totalDifferingInfoCards = await _differingInfoCards.CountAsync();
            var totalChangeQualButtons = await _changeQualificationButtons.CountAsync();

            Assert.That(totalQualHeadings, Is.EqualTo(2));
            Assert.That(totalDifferingInfoCards, Is.EqualTo(2));

            for (int i = 0; i < totalChangeQualButtons; i++)
            {
                bool isChangeButtonEnabled = await _changeQualificationButtons.Nth(i).IsEnabledAsync();
                Assert.IsTrue(isChangeButtonEnabled);
            }
        }

    }
}