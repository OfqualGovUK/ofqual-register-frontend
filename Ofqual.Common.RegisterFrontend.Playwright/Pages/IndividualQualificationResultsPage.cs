using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests.Pages
{
public class IndividualQualificationResultsPage : PageTest
{
    private readonly IPage _page;
    private readonly ILocator _heading;
    public IndividualQualificationResultsPage(IPage Page){
        _page = Page;
        _heading = Page.Locator("h1");
    }

    public async Task checkPageHeading(String heading){
        await Expect(_heading).ToHaveTextAsync(heading);
        }

    }
}