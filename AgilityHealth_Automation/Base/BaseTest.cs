using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Benchmarking;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;

namespace AgilityHealth_Automation.Base
{
    [TestClass]
    public class BaseTest
    {
        public TestContext TestContext { get; set; }
        public static TestEnvironment TestEnvironment;
        protected IWebDriver Driver;
        protected Logger Log { get; set; }
        protected Browser B = new Browser();
        protected FileUtil FileUtil = new FileUtil();
        public static User User; // can be CA ,TA, OL, BL, SA, or PA
        public static User InsightsUser;
        public static string ApplicationUrl;
        public static Company Company;
        public static Dictionary<string, double> PageLoadTime;
        public static readonly object ClipboardLock = new object();

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            //Setting Environment / Browser and dependent variables
            TestEnvironment = new TestEnvironment(context);
            PageLoadTime = new Dictionary<string, double>();

            //Setting company object
            Company = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);

            //Clearing up Environment
            //Database Clean up
            if (TestEnvironment.Parameters["CleanCompany"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                try
                {
                    DataBaseUtil.CleanCompanyForAutomation(Company.Id, TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"**********\nCleanCompanyForAutomation({Company.Id}) failed.\nConnection string: <{TestEnvironment.DatabaseConnectionString}>\nException:{e.Message} \n**********");
                }
            }

            if (TestEnvironment.Parameters["CleanNTierCompany"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                try
                {
                    DataBaseUtil.CleanCompanyForAutomation(Company.NtierId, TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"**********\nCleanCompanyForAutomation({Company.NtierId}).\nConnection string: <{TestEnvironment.DatabaseConnectionString}>\nException:{e.Message} \n**********");
                }
            }

            if (TestEnvironment.Parameters["DeleteTestCompanies"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                try
                {
                    DataBaseUtil.DeleteTestCompanies(TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"**********\nDeleteTestCompanies failed.\nConnection string: <{TestEnvironment.DatabaseConnectionString}>\nException:{e.Message} \n**********");
                }
            }

            if (TestEnvironment.Parameters["PopulateCompanyTeamHierarchy"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                try
                {
                    DataBaseUtil.PopulateCompanyTeamHierarchy(TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"**********\nPopulateCompanyTeamHierarchy failed.\nConnection string: <{TestEnvironment.DatabaseConnectionString}>\nException:{e.Message} \n**********");
                }
            }


            ApplicationUrl = $"https://{TestEnvironment.EnvironmentName}.agilityinsights.ai";
            User = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (!TestEnvironment.NewNav)
            {
                InsightsUser = (User.IsSiteAdmin() || User.IsPartnerAdmin())
                    ? User : TestEnvironment.UserConfig.GetUserByDescription("insights user");
            }
            if (User.IsMember())
            {
                Constants.TeamMemberEmail1 = User.Username;
                SharedConstants.TeamMember1.Email = User.Username;
            }

            // Delete old log files
            var fileUtil = new FileUtil();
            fileUtil.DeleteAllFilesOlderThan(Constants.LogRetentionDays, $"{fileUtil.GetBasePath()}Resources\\Logs", ".log");
            fileUtil.DeleteAllFilesOlderThan(Constants.LogRetentionDays, $"{fileUtil.GetBasePath()}Resources\\Screenshots", ".png");

            // try to get an api token
            try
            {
                ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Unable to get token for first login");
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            //Creating an excel file to store page vs load time
            ExcelUtil.CreateExcel(PageLoadTime);
        }

        [TestInitialize]
        public void Setup()
        {
            Log = new Logger($"{FileUtil.GetBasePath()}Resources\\Logs\\{SetFileName("Log")}.log");
            Log.Info($"Starting test {TestContext.TestName}");
            Driver = B.SetUp(TestEnvironment);
            
        }


        [TestCleanup]
        public void Teardown()
        {
            Log.Info($"Result - {TestContext.TestName} {TestContext.CurrentTestOutcome}");

            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                try
                {
                    var path = $"{FileUtil.GetBasePath()}Resources\\Screenshots\\{SetFileName("IMG")}.png";
                    Driver.TakeScreenShot(path);
                    TestContext.AddResultFile(path);
                    
                    CaptureBrowserLog();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            try
            {
                TestContext.AddResultFile(Log.LogPath);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            B.TearDown();
        }
        
        //Set filename
        private string SetFileName(string type)
        {
            var filename = $"[Test]_[{type}]_{TestContext.TestName}_{DateTime.Now:yy-MM-dd HH.mm.ss}";
            return filename;
        }

        public void CaptureBrowserLog()
        {
            var logs = Driver.Manage().Logs.GetLog(LogType.Browser);
            foreach (var log in logs)
            {
                if (log.Level != LogLevel.Severe) continue;
                Log.Info($"[URL]: {Driver.GetCurrentUrl()}");
                Log.Info($"[BROWSER LOG]: {log.Message}");
            }
        }

        protected static void VerifySetup(bool classInitializationFailed, string message = "")
        {
            if (classInitializationFailed) throw new Exception($"ClassInitialize was not successful. Aborting test {message}");
        }

        public void TakeFullPageScreenShot(string filePath, int timeout=5000)
        {
            Log.Step(nameof(BenchmarkingPopUp), "Take the full webpage screen shot.");
            Thread.Sleep(timeout);//Some dashboard takes time to load

            FileUtil.DeleteFilesInDownloadFolder($"{filePath}");
            var vcs = new VerticalCombineDecorator(new ScreenshotMaker());
            var screen = Driver.TakeScreenshot(vcs);

            using var image = Image.FromStream(new MemoryStream(screen));
            image.Save(filePath, ImageFormat.Png);
        }

        public void LoginToProductionEnvironment(string env)
        {
            var login = new LoginPage(Driver, Log);
            if (Constants.NonSsoEnvironments.Contains(env))
            {
                login.NavigateToLoginPage(env);
            }
            else if (Constants.AgilityInsightsSaDomain.Contains(env)) 
            {
                if (env.Equals("rcmc", StringComparison.OrdinalIgnoreCase))
                {
                    login.NavigateToSaDomainRemoteLoginPage(env);
                }
                else
                {
                    login.NavigateToSaDomainLoginPage(env);
                }
            }
            else
            {
                login.NavigateToRemoteLoginPage(env);
            }
            login.LoginToApplication(TestEnvironment.ProdEmail,TestEnvironment.ProdPassword);
        }
    }
}

