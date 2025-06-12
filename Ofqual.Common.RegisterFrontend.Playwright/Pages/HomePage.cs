using Microsoft.Playwright;

namespace Ofqual.Common.RegisterFrontend.Playwright.Pages;

public class HomePage : BasePage
{
    private readonly ILocator _qualificationsLink;
    private readonly ILocator _organisationsLink;

    public HomePage(IPage page) : base(page)
    {
        _qualificationsLink = page.GetByRole(AriaRole.Link, new() { Name = "Find a regulated qualification" });
        _organisationsLink = page.GetByRole(AriaRole.Link, new() { Name = "Find a regulated awarding organisation" });
    }
    public async Task GoToHomePage()
    {
        await _page.GotoAsync($"{_baseUrl}");
    }

    public async Task clickFindQualificationsLink()
    {
        await _qualificationsLink.ClickAsync();
    }

    public async Task clickFindOrganisationsLink()
    {
        await _organisationsLink.ClickAsync();
    }
}