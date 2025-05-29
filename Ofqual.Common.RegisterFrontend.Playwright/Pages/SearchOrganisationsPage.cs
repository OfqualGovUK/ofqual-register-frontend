using Microsoft.Playwright;


namespace Ofqual.Common.RegisterFrontend.Playwright.Pages;

public class SearchOrganisationsPage : BasePage
{
    private readonly ILocator _searchOrganisationField;
    private readonly ILocator _searchOrganisationsButton;
    private readonly ILocator _showAllOrganisationsButton;

    public SearchOrganisationsPage(IPage page) : base(page)
    {
        _searchOrganisationField = page.Locator("#name");
        _searchOrganisationsButton = page.GetByRole(AriaRole.Button, new() { Name = "Search organisation" });
        _showAllOrganisationsButton = page.GetByRole(AriaRole.Button, new() { Name = "Show all organisations" });
    }

    public async Task EnterOrganisationDetails(String organisationNumber)
    {
        await _searchOrganisationField.FillAsync(organisationNumber);
    }

    public async Task ClickSearchOrganisation()
    {
        await _searchOrganisationsButton.ClickAsync();
    }

    public async Task ClickShowAllOrganisations()
    {
        await _showAllOrganisationsButton.ClickAsync();
    }

}