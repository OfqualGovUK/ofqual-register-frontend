using Microsoft.Playwright;
using Newtonsoft.Json.Linq;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages;

public abstract class BasePage : PageTest
{
    protected readonly IPage _page;
    protected readonly ILocator _heading;
    protected readonly string _baseUrl;
    protected readonly ILocator _cards;

    protected BasePage(IPage page)
    {
        _page = page;
        _baseUrl = Environment.GetEnvironmentVariable("RegisterBaseUrl") ?? "http://localhost:5224/";
        _heading = page.Locator("h1");
        _cards = page.Locator(".govuk-summary-card");
    }
    public async Task CheckPageHeading(string heading)
    {
        await Expect(_heading).ToHaveTextAsync(heading);
    }

    public async Task VerifyCardsDetails(String expectedJson)
    {
        var actualJson = new JObject();
        var cards = await _cards.AllAsync();

        foreach (var card in cards)
        {
            string cardTitle = await card.Locator("h2.govuk-summary-card__title").TextContentAsync();

            var cardData = new JObject();

            var rows = await card.Locator(".govuk-summary-list__row").AllAsync();
            foreach (var row in rows)
            {
                string key = await row.Locator(".govuk-summary-list__key").TextContentAsync();
                string value = await row.Locator(".govuk-summary-list__value").TextContentAsync();
                cardData[key.Trim()] = value.Trim();
            }

            actualJson[cardTitle] = cardData;
        }

        string actualJsonString = actualJson.ToString();
        JToken actual = JToken.Parse(actualJsonString);
        JToken expected = JToken.Parse(expectedJson);

        bool areJsonsEqual = JToken.DeepEquals(actual, expected);
        Assert.IsTrue(false);
    }

}