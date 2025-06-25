using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    public class ManageRadarBaseTests : BaseTest
    {
        protected void DeleteRadar(string radarName)
        {
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            try
            {
                var attempt = 0;
                manageRadarPage.NavigateToPage();
                manageRadarPage.ClickOnDeleteRadarIcon(radarName);
                manageRadarPage.DeleteAssessmentPopUpClickOnDeleteButton();
                while (manageRadarPage.IsRadarPresent(radarName) && attempt < 5)
                {
                    manageRadarPage.ClickOnDeleteRadarIcon(radarName);
                    manageRadarPage.DeleteAssessmentPopUpClickOnDeleteButton();
                    attempt++;
                }
            }
            catch
            {
                Log.Info($"'{radarName}' radar was not deleted");
            }
        }


    }
}