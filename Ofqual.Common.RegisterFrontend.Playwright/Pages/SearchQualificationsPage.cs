using Microsoft.Playwright;


namespace PlaywrightTests.Pages
{
public class SearchQualificationsPage : PageTest
{
    private readonly IPage _page;
    private readonly ILocator _searchQualificationField;
    private readonly ILocator _searchQualificationsButton;
    public SearchQualificationsPage(IPage Page){
        _page = Page;
        _searchQualificationField = Page.Locator("#title");
        _searchQualificationsButton = Page.GetByRole(AriaRole.Button, new() { Name = "Search qualifications" });
    }

    public async Task enterQualificationNumber(String qualificationNumber){
        await _searchQualificationField.FillAsync(qualificationNumber);        
    }

    public async Task clickSearchQualifications(){
        await _searchQualificationsButton.ClickAsync();
    }

}
}