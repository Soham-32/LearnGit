using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.PageObjects.PowerBi;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.EnterpriseAgility
{
    [TestClass]
    [TestCategory("PowerBIAuditRefresh")]
    public class PowerBiRefreshAuditTests : BaseTest
    {
        [DataTestMethod]
        [DataRow("7-Eleven")]
        [DataRow("A Global Health Company")]
        [DataRow("A1 Finance")]
        [DataRow("ABSA")]
        [DataRow("ABSA CIB (IQB)")]
        [DataRow("Accenture")]
        [DataRow("AgilityHealth")]
        [DataRow("Alithya")]
        [DataRow("AllState Canada (Atos)")]
        [DataRow("Ameriprise")]
        [DataRow("Amtrak")]
        [DataRow("Amway")]
        [DataRow("AstraZeneca")]
        [DataRow("ATI Playground")]
        [DataRow("Atlassian")]
        [DataRow("Atos")]
        [DataRow("Australian Taxation Office")]
        [DataRow("Banco Pichincha")]
        [DataRow("BCBS of Louisiana")]
        [DataRow("BCI")]
        [DataRow("Bloomberg")]
        [DataRow("Bloomberg Sandbox")]
        [DataRow("Blue Cross Blue Shield of MA")]
        [DataRow("Bread Financial")]
        [DataRow("Cardinal Health")]
        [DataRow("CDW (Atkco)")]
        [DataRow("Citi")]
        [DataRow("Citi Sandbox")]
        [DataRow("Citizens Property Insurance Corporation")]
        [DataRow("Cognizant")]
        [DataRow("College of Western Idaho")]
        [DataRow("CS - NFL")]
        [DataRow("Cummins")]
        [DataRow("CVS - COPY2")]
        [DataRow("CVS Health")]
        [DataRow("Dave Test")]
        [DataRow("DBM")]
        [DataRow("DESIRED DEFAULTS COMPANY")]
        [DataRow("DTCC")]
        [DataRow("DTCC Sandbox")]
        [DataRow("Edward Jones")]
        [DataRow("Electronic Toll Company (IQBusiness)")]
        [DataRow("Elevance Health")]
        [DataRow("Elevance Health Sandbox 1")]
        [DataRow("Eliassen Group")]
        [DataRow("EPiC Agile")]
        [DataRow("Equifax")]
        [DataRow("Erie Insurance Group")]
        [DataRow("Erie Insurance Group Sandbox")]
        [DataRow("Fairway Independent Mortgage")]
        [DataRow("FBI")]
        [DataRow("Federal Reserve")]
        [DataRow("Flexential")]
        [DataRow("Focus on the Family")]
        [DataRow("Geico")]
        [DataRow("GSK")]
        [DataRow("Honeywell")]
        [DataRow("Honeywell Sandbox")]
        [DataRow("IGM Wealth Management")]
        [DataRow("Insperity")]
        [DataRow("Invesco (Cognizant)")]
        [DataRow("IQBusiness")]
        [DataRow("IQbusiness (EU)")]
        [DataRow("IQBusiness (Internal)")]
        [DataRow("ITX Corp")]
        [DataRow("Jabil")]
        [DataRow("Jolie TEST Company")]
        [DataRow("JolieTonyTest")]
        [DataRow("Jordan's Test Company")]
        [DataRow("Merck")]
        [DataRow("Mindex")]
        [DataRow("Ministry of Health")]
        [DataRow("Money and Pension Services (MaPS)")]
        [DataRow("Montgomery Wards")]
        [DataRow("NASA-MSFC")]
        [DataRow("New York Life")]
        [DataRow("Nikki's Awesome Company")]
        [DataRow("NN Group - Hungary")]
        [DataRow("NN Group - Japan")]
        [DataRow("NN Group - Poland")]
        [DataRow("NN Group - Sandbox")]
        [DataRow("NN Group - Spain")]
        [DataRow("Norfolk Southern")]
        [DataRow("Northern Trust")]
        [DataRow("Northrop Grumman")]
        [DataRow("NTT (Internal Use)")]
        [DataRow("Paychex")]
        [DataRow("Piyush Company")]
        [DataRow("Prudential Financial Sandbox")]
        [DataRow("Prudential Financial, Incorporated")]
        [DataRow("Prudential Financial, Incorporated - PGIM")]
        [DataRow("Quad")]
        [DataRow("Quest Analytics")]
        [DataRow("Raytheon")]
        [DataRow("Raytheon Sandbox")]
        [DataRow("RCMC")]
        [DataRow("Reef Consulting")]
        [DataRow("Royal Bank of Canada")]
        [DataRow("S&P DJI")]
        [DataRow("Saudi Red Crescent")]
        [DataRow("Schneider Electric")]
        [DataRow("Scotiabank")]
        [DataRow("SendoAgil")]
        [DataRow("Shell")]
        [DataRow("Shelter Insurance")]
        [DataRow("Society Insurance")]
        [DataRow("Starbucks")]
        [DataRow("Sylvamo")]
        [DataRow("Tomahawk Training Company")]
        [DataRow("Truist Bank")]
        [DataRow("Truist Sandbox")]
        [DataRow("UEFA ICT")]
        [DataRow("Ultralight Solutions - App")]
        [DataRow("USAF")]
        [DataRow("Waters Corp")]
        [DataRow("Wellmark (Cognizant)")]
        [DataRow("Wellmark BCBS")]
        [DataRow("World Health Organization (WHO)")]

        public void PowerBi_AuditRefresh_VerifyRefreshTime(string companyName)
        {
            var refreshAuditPage = new AuditRefreshPage(Driver, Log);

            Log.Info("Navigate to the Microsoft login page");
            refreshAuditPage.NavigateToPage();

            Log.Info("Login to PowerBi Platform");
            refreshAuditPage.LoginToPowerBiDashboard(TestEnvironment.PowerBiEmail, TestEnvironment.PowerBiPassword);

            Log.Info("Navigate to Audit page and verify time stamps");
            refreshAuditPage.NavigateToReportsPage();

            refreshAuditPage.SelectCompanyFromCompanyDropdown(companyName);
            var companyRefreshDateAndTime = refreshAuditPage.GetRefreshedDateAndTime(companyName);

            for (var i = 0; i < companyRefreshDateAndTime.Count - 1; i++)
            {
                var dateTimeForCurrentRefresh = DateTime.ParseExact(companyRefreshDateAndTime[i], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                var dateTimeForPastRefresh = DateTime.ParseExact(companyRefreshDateAndTime[i + 1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                var differenceInMinutes = (dateTimeForCurrentRefresh - dateTimeForPastRefresh).TotalMinutes;
                var dateTimeVariable = differenceInMinutes >= 55 && differenceInMinutes <= 65;
                Assert.IsTrue(dateTimeVariable, $"{companyRefreshDateAndTime[i]} is not refreshed in 65 min or got refreshed in less than 55 min for {companyName}");
            }

            var currentCstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var differenceOfTime = currentCstTime - DateTime.ParseExact(companyRefreshDateAndTime.FirstOrDefault(),
                "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            Assert.IsTrue(differenceOfTime.TotalMinutes < 65,"Last Refresh was done before 65 min");
        }
    }
}
