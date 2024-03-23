using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace PLM.Library.Tests;



public class FrontendTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl;

    public FrontendTests()
    {
        _baseUrl = "https://localhost:7159/";
        _driver = new ChromeDriver();
    }

    [Fact]
    public void TestHomePageTitle()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        Assert.Equal("Parking Lot", _driver.Title);
    }

    [Fact]
    public void TestTagNumberInput()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        IWebElement tagNumberInput = _driver.FindElement(By.Id("tagNumber"));

        Assert.NotNull(tagNumberInput);
    }

    [Fact]
    public void TestCheckInButton()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        IWebElement checkInButton = _driver.FindElement(By.Id("checkInBtn"));

        Assert.NotNull(checkInButton);
    }

    [Fact]
    public void TestCheckOutButton()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        IWebElement checkOutButton = _driver.FindElement(By.Id("checkOutBtn"));

        Assert.NotNull(checkOutButton);
    }

    [Fact]
    public void TestTablePresence()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        IWebElement table = _driver.FindElement(By.Id("parkingSnapshotTable"));

        Assert.NotNull(table);
    }

    [Fact]
    public void TestTableContent()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        ReadOnlyCollection<IWebElement> tableRows = _driver.FindElements(By.CssSelector("#parkingSnapshotTable tr"));

        Assert.True(tableRows.Count >= 1);
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
