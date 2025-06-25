using AtCommon.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;

namespace AgilityHealth_Automation.Utilities
{
    public class Browser
    {

        private IWebDriver Driver;
        private ChromeDriver driver;

        //Setup up method
        public IWebDriver SetUp(TestEnvironment testEnvironment)
        {

            if (testEnvironment.Browser.ToLower().Equals("chrome"))
            {
                var options = new ChromeOptions();
                options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);

                Driver = new ChromeDriver(options);
            }
            else if (testEnvironment.Browser.ToLower().Equals("edge"))
            {
                Driver = new EdgeDriver();
            }
            else
            {
                Driver = new ChromeDriver();
            }

            Driver.Manage().Cookies.DeleteAllCookies();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(180);
            Driver.Manage().Timeouts().AsynchronousJavaScript = testEnvironment.JsTimeout;
            Driver.Manage().Window.Maximize();
            return Driver;

        }

        public ChromeDriver GeoLocationSetUp(TestEnvironment testEnvironment)
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(180);
            driver.Manage().Timeouts().AsynchronousJavaScript = testEnvironment.JsTimeout;
            driver.Manage().Window.Maximize();

            // Use Chrome DevTools Protocol to set geolocation override
            driver.ExecuteCdpCommand("Emulation.setGeolocationOverride", SharedConstants.GetRandomCityLatAndLong());
            return driver;
        }


        //Teardown method
        public void TearDown()
        {
            if (Driver != null)
            {
                try
                {
                    Driver.Close();
                    Driver.Quit();
                }
                catch
                {
                    Driver.Quit();
                }
            }
        }
    }
}
