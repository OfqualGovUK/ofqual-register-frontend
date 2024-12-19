using System.Reflection;
using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.Pages;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class QualificationSearch: PageTest
{
    private List<TestData> _testData;

    [SetUp]
    public void Setup()
    {
        var jsonFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testData.json");
        if (File.Exists(jsonFilePath))
        {
            var json = File.ReadAllText(jsonFilePath);
            var testDataCollection = JsonSerializer.Deserialize<TestDataCollection>(json);
            _testData = testDataCollection?.Tests ?? new List<TestData>();
        }
        else
        {
            throw new FileNotFoundException($"The file {jsonFilePath} was not found.");
        }
    }

    [Test]
    public async Task SearchQualificationNumberWithSlashes()
    {
        var testData = _testData[0];
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await Page.GotoAsync("http://localhost:5224/");
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.enterQualificationNumber(testData.QualificationNumber);
        await searchQualificationsPage.clickSearchQualifications();
        await individualQualificationResultsPage.checkPageHeading(testData.ExpectedHeading);
    }

    [Test]
    public async Task SearchQualificationNumberWithoutSlashes()
    {
        var testData = _testData[1];
        var homePage = new HomePage(Page);
        var searchQualificationsPage = new SearchQualificationsPage(Page);
        var individualQualificationResultsPage = new IndividualQualificationResultsPage(Page);

        await Page.GotoAsync("http://localhost:5224/");
        await homePage.clickFindQualificationsLink();
        await searchQualificationsPage.enterQualificationNumber(testData.QualificationNumber);
        await searchQualificationsPage.clickSearchQualifications();
        await individualQualificationResultsPage.checkPageHeading(testData.ExpectedHeading);
    }

    public class TestData
    {
        public string QualificationNumber { get; set; }
        public string ExpectedHeading { get; set; }
    }

    public class TestDataCollection
    {
        public List<TestData> Tests { get; set; }
    }
}
