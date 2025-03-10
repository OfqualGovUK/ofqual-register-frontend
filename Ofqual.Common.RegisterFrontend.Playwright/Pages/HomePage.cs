using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
public class HomePage : PageTest
{
    private readonly IPage _page;
    private readonly ILocator _qualificationsLink;
    private readonly string _baseUrl;
    public HomePage(IPage Page)
    {
        _page = Page;
        _qualificationsLink = Page.GetByRole(AriaRole.Link, new() { Name = "Find a regulated qualification" });
        _baseUrl = Environment.GetEnvironmentVariable("RegisterBaseUrl") ?? "http://localhost:7159/";
    }
    public async Task GoToHomePage()
    {
        await _page.GotoAsync(_baseUrl);
    }

    public async Task clickFindQualificationsLink()
    {
        await _qualificationsLink.ClickAsync();        
    }

}
}