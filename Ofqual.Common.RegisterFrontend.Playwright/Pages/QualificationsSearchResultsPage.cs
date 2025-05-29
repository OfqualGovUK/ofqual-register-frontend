using Microsoft.Playwright;
using Newtonsoft.Json.Linq;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages;

public class QualificationsSearchResultsPage : BasePage
{
    private readonly ILocator _filterCategory;
    private readonly ILocator _qualificationCards;
    private readonly ILocator _paginationSection;
    private readonly ILocator _checkboxes;
    private readonly ILocator _compareButton;

    public QualificationsSearchResultsPage(IPage page) : base(page)
    {
        _filterCategory = page.Locator(".app-filter__selected h4");
        _qualificationCards = page.Locator(".app-application-cards .app-application-card");
        _paginationSection = page.Locator(".govuk-pagination");
        _checkboxes = page.Locator(".app-application-card .govuk-checkboxes__input");
        _compareButton = page.Locator("#compareButton");
    }

    public async Task VerifySelectedFilters(string expectedFilters)
    {
        var selectedFiltersJson = new JObject();
        var filterCategories = await _filterCategory.AllAsync();

        foreach (var category in filterCategories)
        {
            string categoryTitle = await category.TextContentAsync();

            var ulLocator = category.Locator("xpath=following-sibling::ul").First;
            var filterValues = await ulLocator.Locator("a.app-filter__tag").AllAsync();

            var valuesList = new List<string>();

            foreach (var filter in filterValues)
            {
                string filterText = (await filter.TextContentAsync())?.Trim();
                if (!string.IsNullOrEmpty(filterText))
                {
                    valuesList.Add(filterText.Replace("Remove this filter", "").Trim());
                }
            }

            selectedFiltersJson[categoryTitle] = new JArray(valuesList);
        }
        string selectedFiltersString = selectedFiltersJson.ToString();
        JToken actual = JToken.Parse(selectedFiltersString);
        JToken expected = JToken.Parse(expectedFilters);

        bool areJsonsEqual = JToken.DeepEquals(actual, expected);
        Assert.IsTrue(areJsonsEqual);
    }

    public async Task VerifyQualificationsCountAndPagination(int expectedQualificationsPerPage)
    {
        var actualQualificationsCount = await _qualificationCards.CountAsync();
        bool isPaginationDisplayed = await _paginationSection.IsVisibleAsync();

        Assert.That(actualQualificationsCount, Is.EqualTo(expectedQualificationsPerPage));
        Assert.IsTrue(isPaginationDisplayed);
    }

    public async Task ClickToViewFirstQualificationDetails()
    {
        await _qualificationCards.First.Locator("a").ClickAsync();
    }

    public async Task SelectQualificationsCheckboxesAndCompare(int checkboxCount)
    {
        for( int i=0; i< checkboxCount; i++)
        {
            await _checkboxes.Nth(i).ClickAsync();
        }
        await _compareButton.ClickAsync();
    }

}