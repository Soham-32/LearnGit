using ImageMagick;
using OpenQA.Selenium;
using AtCommon.Utilities;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;
using System.Text.RegularExpressions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility
{
    public class EnterpriseAgilityPage : InsightsDashboardPage
    {
        public EnterpriseAgilityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        //Objects
        private readonly FileUtil FileUtil = new FileUtil();

        //Locators

        private readonly By Iframe = By.XPath("(//iframe[contains(@src,'https://app.powerbi.com/report')])[2]");

        private readonly By LoadingSpinner = By.CssSelector("div.spinner");

        private readonly By EnterpriseAgilityTabText = By.XPath("//button[text()='Enterprise Agility'] | //button//*[text()='Enterprise Agility']");

        private readonly By Insights4LenzDashboardTabText = By.XPath("//button[text()='4 Lenz Dashboard']");

        private readonly By AllActiveTeamCount = By.CssSelector("div[id^='highcharts'] text[text-anchor='middle'][data-z-index='5']");

        private readonly By RefreshSyncDate = AutomationId.Equals("last_sync");

        private readonly By DownloadPdfButton = By.XPath("//button[@title='Download as PDF']");

        private readonly By RadarGrowthJourneyDropdown = By.XPath("//div[@aria-label='Growth Journey']");

        private readonly By PerformanceMetricsChart = By.XPath("(//*[local-name()='svg' and @name='Stacked bar chart'])[1]");

        private readonly By TeamGrowthItemsTooltipIcon = By.XPath("//div[contains(@aria-label, 'Back . Team Growth Items')]");

        private readonly By GrowthItemCompletionOverTimeCount = By.XPath("//*[local-name()='svg' and contains(@aria-label, 'GI_Done_Status_Count ')]");
        
        private readonly By ConfirmationPopup = By.Id("confirmation-dialog-title");

        private readonly By ConfirmButton = By.XPath("//button[text()='Confirm']");
        
        private readonly By ReloadNowButton = By.XPath("//button[text()='Reload Now']");
        private readonly By TitleOfDetailsPopup = By.XPath("//h2[contains(text(),'Selected Team')]");
        private readonly By TeamDetailsPopupCloseButton = By.XPath("//h2[contains(text(),'Selected Team')]/following-sibling::button");

        private static By LinksToListOfTeams(string WidgetTitle) => By.XPath($"//span[text()='{WidgetTitle}']//parent::div//div[@class='susSpan']/span");
        private static By LeftNavCompanyName(string companyName) => By.XPath($"//div[contains(@title, '{companyName}')]");

        //Methods
        //Iframe
        public void SwitchToIFrame()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Switch to the enterprise agility iframe");
            Driver.SwitchToDefaultIframe();
            Driver.SwitchToFrame(Wait.UntilElementExists(Iframe));
        }
        public string GetEnterpriseAgilityTabText()
        {
            return Wait.UntilElementVisible(EnterpriseAgilityTabText).GetText();
        }

        public void WaitTillLoadingSpinnersExists()
        {
            Wait.UntilElementNotExist(LoadingSpinner, 25);
        }
        public string Get4LenzDashboardTabText()
        {
            return Wait.UntilElementVisible(Insights4LenzDashboardTabText).GetText();
        }

        public void NavigateToInsightsEnterpriseAgilityTabPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/insights/company/{companyId}/parents/0/team/0/tab/enterpriseagility");
        }

        public void NavigateToInsights4LenzDashboardTabPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/insights/company/{companyId}/parents/0/team/0/tab/performancedashboard");
        }

        public string GetAllActiveTeamsCount()
        {
            return Wait.UntilElementVisible(AllActiveTeamCount).GetText();
        }

        public string GetRefreshSyncDate()
        {
            return Wait.UntilElementVisible(RefreshSyncDate).GetText();
        }

        public bool IsDownloadPdfButtonEnabled()
        {
            return Wait.UntilElementEnabled(DownloadPdfButton).Displayed;
        }

        public bool IsNavigatedCompanyDisplayed(string companyName)
        {
            Wait.HardWait(3000); //Takes time to load the navigated company name.
            return Wait.UntilElementVisible(LeftNavCompanyName(companyName)).Displayed;
        }

        public void ScrollAndTakeScreenShotOfEnterpriseAgilityDashboard(string companyName)
        {
            Log.Step(nameof(EnterpriseAgilityPage), $"Scroll to each widget for screenshot {companyName}");
            List<string> filePaths = new List<string>
            {
            $"{FileUtil.GetDownloadPath()}\\{companyName}Row1.png",
            $"{FileUtil.GetDownloadPath()}\\{companyName}Row2.png",
            $"{FileUtil.GetDownloadPath()}\\{companyName}Row3.png",
            $"{FileUtil.GetDownloadPath()}\\{companyName}Row4.png",
            $"{FileUtil.GetDownloadPath()}\\{companyName}Row5.png"
            };

            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            Driver.SwitchToFrame(Iframe);
            js.ExecuteScript("document.body.style.transform = 'scale(0.85)'");

            for (int i = 0; i < filePaths.Count; i++)
            {
                string filepath = filePaths[i];

                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        Driver.JavaScriptScrollToElement(RadarGrowthJourneyDropdown);
                        break;
                    case 2:
                        Driver.JavaScriptScrollToElementCenter(PerformanceMetricsChart);
                        break;
                    case 3:
                        Driver.JavaScriptScrollToElement(TeamGrowthItemsTooltipIcon);
                        break;
                    case 4:
                        Driver.JavaScriptScrollToElement(GrowthItemCompletionOverTimeCount);
                        break;
                }

                // Take a screenshot
                Wait.HardWait(2000);
                Screenshot screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                FileUtil.DeleteFilesInDownloadFolder($"{companyName}Row{i + 1}.png");
                screenshot.SaveAsFile(filepath);
            }
            // Merge all row screenshot in one image
            using (MagickImageCollection collection = new MagickImageCollection())
            {
                foreach (string filepath in filePaths)
                {
                    collection.Add(new MagickImage(filepath));
                }
                using (MagickImage result = collection.AppendVertically() as MagickImage)
                {
                    if (result != null)
                    {
                        result.Write(
                            $"{FileUtil.GetBasePath()}\\Resources\\EnterpriseAgilityDashboardScreenshots\\{companyName}.png");
                    }
                }
            }
        }
        public void ClickOnDownloadPdfButton()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Click on the 'DownloadPDF' button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DownloadPdfButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsConfirmationPopupDisplayed()
        {
            return Wait.UntilElementVisible(ConfirmationPopup).Displayed;
        }

        public void ClickOnConfirmButton()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Click on the 'Confirm' button");
            Wait.UntilElementClickable(ConfirmButton).Click();
        }
        public void ClickOnReloadNowButton()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Click on the 'Reload now' button");
            Wait.UntilElementVisible(ReloadNowButton).Click();
        }
        public void ClickOnListOfTeamsPopupCloseButton()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Click on the 'Reload now' button");
            Wait.UntilElementVisible(TeamDetailsPopupCloseButton).Click();
        }

        public string GetTeamsCountAndClickOnDetailPopupLink(string WidgetTitle)
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Click on the 'Teams' link to open the popup");
            var teamCountInWidget = Wait.UntilElementVisible(LinksToListOfTeams(WidgetTitle)).GetText().Split(' ')[0];
            Wait.UntilElementVisible(LinksToListOfTeams(WidgetTitle)).Click();
            Wait.HardWait(1000); // Wait till the data is loaded inside the popup
            return teamCountInWidget;
        }
        public string GetTeamCountFromTheTitle()
        {
            Log.Step(nameof(EnterpriseAgilityPage), "Get the count of teams by extracting it from title");
            return Regex.Match(Wait.UntilElementVisible(TitleOfDetailsPopup).GetText(), @"\((\d+)\)").Groups[1].Value;
        }

        public static List<string> GetEnterpriseAgilityWidgetTitles()
        {
            return new List<string>
            {
                "All Active Teams",
                "Assessment Count",
                "Radar Usage",
                "Agility Over Time",
                "Maturity Over Time",
                "Performance Over Time",
                "Agility Index",
                "Maturity Metrics",
                "Performance Metrics",
                "Team Growth Items",
                "Organizational Growth Items",
                "Enterprise Growth Items",
                "Growth Item Completion Over Time"
            };
        }


    }
}