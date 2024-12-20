using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
public class HomePage : PageTest
{
    private readonly IPage _page;
    private readonly ILocator _qualificationsLink;
    public HomePage(IPage Page){
        _page = Page;
        _qualificationsLink = Page.GetByRole(AriaRole.Link, new() { Name = "Find a regulated qualification" });
    }

    public async Task clickFindQualificationsLink(){
        await _qualificationsLink.ClickAsync();        
    }

}
}