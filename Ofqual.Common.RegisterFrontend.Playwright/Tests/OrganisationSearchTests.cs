using Ofqual.Common.RegisterFrontend.Playwright.Pages;

namespace Ofqual.Common.RegisterFrontend.Playwright.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class OrganisationSearch : PageTest
{
    [Test]
    [Category("RunOnlyThis")]
    public async Task SearchOrganisationName()
    {
        string expectedJson = "{ \"Contact details\": { \"Email\":\"examsofficers@example.comd\", \"Website\": \"AQA Education\", \"Pricing and fees\": \"Qualification fees and prices\" }, " +
                              "\"Ofqual recognition\": { \"Status\": \"Recognised\", \"Recognised from\": \"01 April 2010\", \"Recognition number\": \"RN5196\" }, " +
                              "\"CCEA Regulation recognition\": { \"Status\": \"Recognised\", \"Recognised from\": \"01 April 2010\", \"Recognition number\": \"RN5196\" }, " +
                              "\"Organisation details\": { \"Name\": \"AQA Education DEMO2.2\", \"Legal name\": \"AQA Education\", \"Known as\": \"AQA Education\" } }";

        var homePage = new HomePage(Page);
        var searchOrganisationsPage = new SearchOrganisationsPage(Page);
        var organisationResultsPage = new OrganisationSearchResultsPage(Page);
        var organisationDetailsPage = new OrganisationDetailsPage(Page);

        await homePage.GoToHomePage();
        await homePage.CheckPageHeading("Register of Regulated Qualifications");
        await homePage.clickFindOrganisationsLink();

        await searchOrganisationsPage.CheckPageHeading("Find a regulated awarding organisation");
        await searchOrganisationsPage.EnterOrganisationDetails("AQA Education");
        await searchOrganisationsPage.ClickSearchOrganisation();

        await organisationResultsPage.CheckPageHeading("Organisation search");
        await organisationResultsPage.VerifySearchCriteria("AQA Education");
        await organisationResultsPage.ClickOrganisationLink();

        await organisationDetailsPage.CheckPageHeading("AQA Education DEMO2.2");
        await organisationDetailsPage.VerifyCardsDetails(expectedJson);

    }

    [Test]
    public async Task SearchOrganisationByEmptyQuery()
    {
        var homePage = new HomePage(Page);
        var searchOrganisationsPage = new SearchOrganisationsPage(Page);
        var organisationResultsPage = new OrganisationSearchResultsPage(Page);
        var organisationDetailsPage = new OrganisationDetailsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindOrganisationsLink();
        await searchOrganisationsPage.EnterOrganisationDetails("");
        await searchOrganisationsPage.ClickSearchOrganisation();

        await organisationResultsPage.CheckPageHeading("Organisation search");
        await organisationResultsPage.VerifyOrgCountAndPagination(15);

    }

    [Test]
    public async Task ShowAllOrganisations()
    {
        var homePage = new HomePage(Page);
        var searchOrganisationsPage = new SearchOrganisationsPage(Page);
        var organisationResultsPage = new OrganisationSearchResultsPage(Page);
        var organisationDetailsPage = new OrganisationDetailsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindOrganisationsLink();
        await searchOrganisationsPage.ClickShowAllOrganisations();

        await organisationResultsPage.CheckPageHeading("Organisation search");
        await organisationResultsPage.VerifyOrgCountAndPagination(15);
    }

    [Test]
    public async Task SearchByOrganisationNumber()
    {
        string expectedJson = "{ \"Contact details\": { \"Email\":\"examsofficers@example.comd\", \"Website\": \"AQA Education\", \"Pricing and fees\": \"Qualification fees and prices\" }, " +
                              "\"Ofqual recognition\": { \"Status\": \"Recognised\", \"Recognised from\": \"01 April 2010\", \"Recognition number\": \"RN5196\" }, " +
                              "\"CCEA Regulation recognition\": { \"Status\": \"Recognised\", \"Recognised from\": \"01 April 2010\", \"Recognition number\": \"RN5196\" }, " +
                              "\"Organisation details\": { \"Name\": \"AQA Education DEMO2.2\", \"Legal name\": \"AQA Education\", \"Known as\": \"AQA Education\" } }";

        var homePage = new HomePage(Page);
        var searchOrganisationsPage = new SearchOrganisationsPage(Page);
        var organisationResultsPage = new OrganisationSearchResultsPage(Page);
        var organisationDetailsPage = new OrganisationDetailsPage(Page);

        await homePage.GoToHomePage();
        await homePage.CheckPageHeading("Register of Regulated Qualifications");
        await homePage.clickFindOrganisationsLink();

        await searchOrganisationsPage.CheckPageHeading("Find a regulated awarding organisation");
        await searchOrganisationsPage.EnterOrganisationDetails("RN5196");
        await searchOrganisationsPage.ClickSearchOrganisation();

        await organisationDetailsPage.CheckPageHeading("AQA Education DEMO2.2");
        await organisationDetailsPage.VerifyCardsDetails(expectedJson);

    }

    [Test]
    public async Task ViewAllQualificationsByOrganisation()
    {
        string expectedJson = "{ \"Availability\": [\"Available to learners\"], \"Organisation\": [\"AQA Education DEMO2.2\"] }";
        int qualificationsPerPage = 15;

        var homePage = new HomePage(Page);
        var searchOrganisationsPage = new SearchOrganisationsPage(Page);
        var organisationResultsPage = new OrganisationSearchResultsPage(Page);
        var organisationDetailsPage = new OrganisationDetailsPage(Page);
        var qualificationResultsPage = new QualificationsSearchResultsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindOrganisationsLink();
        await searchOrganisationsPage.EnterOrganisationDetails("RN5196");
        await searchOrganisationsPage.ClickSearchOrganisation();
        await organisationDetailsPage.clickFindAllQualificationsLink();
        await qualificationResultsPage.VerifySelectedFilters(expectedJson);
        await qualificationResultsPage.VerifyQualificationsCountAndPagination(qualificationsPerPage);
    }
}

