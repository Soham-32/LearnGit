using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageLanguages;
using System;
using System.Linq;
using AtCommon.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;


namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageLanguages
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageLanguages")]
    public class ManageLanguagesTests : BaseTest
    {
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageLanguages_AddRemoveLanguages()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageLanguagesPage = new ManageLanguagesPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver,Log);
            var random = new Random();
            const string radarName = SharedConstants.TeamAssessmentType;
            const string featureName = "Manage Languages";
            const string manageLanguageDescription = "Manage languages. Here you can add languages to enable radar translations.";
            const string manageLanguagePageDescription = "Click on the language name to make it active/inactive. Moving a language to the Active column will add it to the Language drop-down in Manage Radars so that translations for this language can be created.";

            Log.Info("Login to the application and navigate to 'Settings' page.");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            settingsPage.NavigateToPage(Company.Id);

            Log.Info($"Verify the {featureName} button is present on the Settings page.");
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(featureName), $"'{featureName}' button is not present on the Settings page");

            Log.Info($"Verify the {featureName} description is proper on the Settings page.");
            Assert.AreEqual(manageLanguageDescription, settingsPage.GetSettingOptionDescription(featureName),"Feature Description doesn't match");

            Log.Info($"Click on {featureName} button and verify the user redirect to proper URL");
            settingsPage.SelectSettingsOption(featureName);
            Assert.AreEqual(ApplicationUrl + "/languages", Driver.GetCurrentUrl(), $"Incorrect '{featureName}' page URL");

            Log.Info($"Verify the {featureName} page description.");
            var actualPageDescription = manageLanguagesPage.GetPageDescription();
            Assert.AreEqual(manageLanguagePageDescription,actualPageDescription,$"'{featureName}' page description doesn't match");

            Log.Info($"Verify the 'InactiveLanguagesFilter' and 'ActiveLanguagesFilter' textboxes are present on the {featureName} page .");
            Assert.IsTrue(manageLanguagesPage.IsInactiveLanguagesFilterBoxPresent(), $"'InactiveLanguagesFilter' textbox is not present on the {featureName} page");
            Assert.IsTrue(manageLanguagesPage.IsActiveLanguagesFilterBoxPresent(), $"'ActiveLanguagesFilter' textbox is not present on the {featureName} page");

            Log.Info("Verify the all the languages present in 'Inactive' and 'Active' Grid  are Clickable.");
            var allInactiveLanguages = manageLanguagesPage.GetAllInactiveLanguages();
            foreach (var language in allInactiveLanguages)
            {
                Assert.IsTrue(manageLanguagesPage.IsLanguageClickable(language),$"{language} is not clickable.");
            }
            var allActiveLanguages = manageLanguagesPage.GetAllActiveLanguages();
            foreach (var language in allActiveLanguages)
            {
                Assert.IsTrue(manageLanguagesPage.IsLanguageClickable(language), $"{language} is not clickable.");
            }

            Log.Info("Verify Search functionality for 'Inactive' and 'Active' textbox are working properly.");
            var inactiveLanguage = allInactiveLanguages.OrderBy(x => random.Next()).FirstOrDefault();
            var activeLanguage = allActiveLanguages.OrderBy(x => random.Next()).FirstOrDefault();

            manageLanguagesPage.SearchLanguageInInactiveFilterBox(inactiveLanguage);
            manageLanguagesPage.SearchLanguageInActiveFilterBox(activeLanguage);
            allInactiveLanguages = manageLanguagesPage.GetAllInactiveLanguages();
            allActiveLanguages = manageLanguagesPage.GetAllActiveLanguages();

            Assert.AreEqual(1, allInactiveLanguages.Count, "Multiple languages are present");
            Assert.AreEqual(1, allActiveLanguages.Count, "Multiple languages are present");
            Assert.AreEqual(allInactiveLanguages.FirstOrDefault(), inactiveLanguage,"Language doesn't match in Inactive Grid");
            Assert.AreEqual(allActiveLanguages.FirstOrDefault(), activeLanguage, "Language doesn't match in Active Grid");

            Log.Info("Select a random language from 'Inactive' grid and verify is it present in 'Active' grid");
            Driver.RefreshPage();
            var randomLanguage = allInactiveLanguages.OrderBy(x => random.Next()).FirstOrDefault();
            manageLanguagesPage.SelectLanguage(randomLanguage);
            allInactiveLanguages = manageLanguagesPage.GetAllInactiveLanguages();
            allActiveLanguages = manageLanguagesPage.GetAllActiveLanguages();
            Assert.That.ListNotContains(allInactiveLanguages,randomLanguage,$"{randomLanguage} language is still present in Inactive Grid");
            Assert.That.ListContains(allActiveLanguages, randomLanguage, $"{randomLanguage} language is not present in Active Grid");

            Log.Info($"Navigate to the Edit radar details page and verify {randomLanguage} language is present in Header Language dropdown");
            manageRadarPage.NavigateToPage();
            manageRadarPage.ClickOnRadarEditIcon(radarName);
            var radarLanguageDropdownAllValues = editRadarDetailsPage.GetHeaderLanguageDropdownAllValues();
            Assert.That.ListContains(radarLanguageDropdownAllValues,randomLanguage,$"{randomLanguage} language is not present in language dropdown on Edit Radar Details page.");

            Log.Info($"Navigate back to the Manage language page and deselect {randomLanguage} language from 'Active' grid and verify is it present in 'Inactive' Grid ");
            manageLanguagesPage.NavigateToPage();
            manageLanguagesPage.SelectLanguage(randomLanguage);
            allInactiveLanguages = manageLanguagesPage.GetAllInactiveLanguages();
            allActiveLanguages = manageLanguagesPage.GetAllActiveLanguages();
            Assert.That.ListContains(allInactiveLanguages, randomLanguage, $"{randomLanguage} language is not present in Inactive Grid");
            Assert.That.ListNotContains(allActiveLanguages, randomLanguage,
                $"{randomLanguage} language is still present in Active Grid");

            Log.Info($"Navigate back to the Edit radar details page and verify {randomLanguage} language should not present in Header Language dropdown");
            manageRadarPage.NavigateToPage();
            manageRadarPage.ClickOnRadarEditIcon(radarName);
            radarLanguageDropdownAllValues = editRadarDetailsPage.GetHeaderLanguageDropdownAllValues();
            Assert.That.ListNotContains(radarLanguageDropdownAllValues, randomLanguage, $"{randomLanguage} language is  present in language dropdown on Edit Radar Details page.");
        }
    }
}
