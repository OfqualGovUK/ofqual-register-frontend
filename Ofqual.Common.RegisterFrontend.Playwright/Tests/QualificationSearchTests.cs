
using Ofqual.Common.RegisterFrontend.Playwright.Pages;

namespace Ofqual.Common.RegisterFrontend.Playwright.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class QualificationSearch : PageTest
{
    [Test]
    public async Task SearchQualificationWithEmptyQuery()
    {
        int qualificationsPerPage = 15;
        
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var qualificationResultsPage = new QualificationsSearchResultsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.EnterQualificationSearchTerm("");
        await searchQualificationsPage.ClickSearchQualifications();

        await qualificationResultsPage.CheckPageHeading("Qualification search");
        await qualificationResultsPage.VerifyQualificationsCountAndPagination(qualificationsPerPage);
    }

    [Test]
    public async Task ShowAllQualifications()
    {
        int qualificationsPerPage = 15;

        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var qualificationResultsPage = new QualificationsSearchResultsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.ClickShowAllQualifications();

        await qualificationResultsPage.CheckPageHeading("Qualification search");
        await qualificationResultsPage.VerifyQualificationsCountAndPagination(qualificationsPerPage);
    }

    [Test]
    public async Task SearchQualificationWithPartialName()
    {
        int qualificationsPerPage = 15;
        string expectedJson = "{ \"Qualification summary\": { \"Qualification type\": \"Project\", \"Qualification level\": \"Level 1\", \"Qualification number\": \"600/9532/5\", \"Guided learning hours\": \"60\", \"Total qualification time\": \"60\", \"Assessment methods\": \"Coursework, Portfolio of Evidence, Practical Demonstration/Assignment\", \"Specification\": \"View this qualification's specification\", \"Sector subject area\": \"Foundations for learning and life\", \"Grading scale\": \"A*/A/B\" }, " +
            "\"Organisation\": { \"Awarding organisation\": \"AQA Education DEMO2.2\" }, \"Further details\": { \"Grading type\": \"Graded\" }, \"Funding details\": { \"Funding in England\": \"Check if this qualification is funded in England\" }, \"Availability\": { \"Status\": \"Available to learners\", \"National availability\": \"England, Northern Ireland, International\" }, " +
            "\"Northern Ireland\": { \"Available in Northern Ireland\": \"Yes\", \"Approved for school use in Northern Ireland\": \"Check if this qualification is approved for use in schools in Northern Ireland on the Department of Education website\" }, \"Additional information\": { \"Regulation start date\": \"30 May 2013\", \"Operational start date\": \"01 September 2013\", \"European qualification level\": \"Level 2\" } }";

        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var qualificationResultsPage = new QualificationsSearchResultsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await homePage.GoToHomePage();
        await homePage.CheckPageHeading("Register of Regulated Qualifications");
        await homePage.clickFindQualificationsLink();

        await searchQualificationsPage.CheckPageHeading("Find a regulated qualification");
        await searchQualificationsPage.EnterQualificationSearchTerm("AQA");
        await searchQualificationsPage.ClickSearchQualifications();

        await qualificationResultsPage.CheckPageHeading("Qualification search");
        await qualificationResultsPage.VerifyQualificationsCountAndPagination(qualificationsPerPage);
        await qualificationResultsPage.ClickToViewFirstQualificationDetails();

        await individualQualificationResultsPage.CheckPageHeading("AQA Level 1 Foundation Project");
        await individualQualificationResultsPage.VerifyCardsDetails(expectedJson);
    }

    [Test]
    public async Task CompareQualifications()
    {
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var qualificationResultsPage = new QualificationsSearchResultsPage(Page);
        var compareQualificationsPage = new CompareQualificationsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.EnterQualificationSearchTerm("AQA");
        await searchQualificationsPage.ClickSearchQualifications();

        await qualificationResultsPage.SelectQualificationsCheckboxesAndCompare(4);

        await compareQualificationsPage.CheckPageHeading("Compare qualifications");
        await compareQualificationsPage.VerifyComparisionDetails();
    }

    [Test]
    public async Task SearchQualificationNumberWithSlashes()
    {
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await homePage.GoToHomePage(); ;
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.EnterQualificationSearchTerm("100/2548/0");
        await searchQualificationsPage.ClickSearchQualifications();
        await individualQualificationResultsPage.CheckPageHeading("OCR Level 3 Free Standing Mathematics Qualification: Additional Maths");
    }

    [Test]
    public async Task SearchQualificationNumberWithoutSlashes()
    {
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await homePage.GoToHomePage();
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.EnterQualificationSearchTerm("10025480");
        await searchQualificationsPage.ClickSearchQualifications();
        await individualQualificationResultsPage.CheckPageHeading("OCR Level 3 Free Standing Mathematics Qualification: Additional Maths");
    }

}