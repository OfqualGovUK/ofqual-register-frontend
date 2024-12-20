
using PlaywrightTests.Pages;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class QualificationSearch: PageTest
{

    [Test]
    public async Task SearchQualificationNumberWithSlashes()
    {
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await Page.GotoAsync("https://localhost:44320/");
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.enterQualificationNumber("100/2548/0");
        await searchQualificationsPage.clickSearchQualifications();
        await individualQualificationResultsPage.checkPageHeading("OCR Level 3 Free Standing Mathematics Qualification: Additional Maths");
    }

    [Test]
    public async Task SearchQualificationNumberWithoutSlashes()
    {
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await Page.GotoAsync("https://localhost:44320/");
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.enterQualificationNumber("10025480");
        await searchQualificationsPage.clickSearchQualifications();
        await individualQualificationResultsPage.checkPageHeading("OCR Level 3 Free Standing Mathematics Qualification: Additional Maths");
    }

}
