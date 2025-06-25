using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AtCommon.Api;
using System.Globalization;


namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class ObstaclesTabPage : BaseTabPage
    {
        public ObstaclesTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        #region Obstacles

        private readonly By AddNewObstacleButton = By.XPath("//button[text()='Add Obstacle']//*[local-name()='svg' and @data-testid='AddCircleIcon']");
        private static By ObstacleTitle(int rowIndex) => By.XPath($"(//table[contains(@class,'k-table k-grid-table k-table-md')]/tbody/tr[not(normalize-space(@class)='k-table-row k-grid-norecords')])[{rowIndex}]/td[1]//textarea[@aria-invalid='false']");
        private static By ObstacleTitleFieldValue(string title) => By.XPath($"//textarea[text()='{title}']");
        private static By ObstacleDescription(int rowIndex) => By.XPath($"(//table[contains(@class,'k-table k-grid-table k-table-md')]/tbody/tr[not(normalize-space(@class)='k-table-row k-grid-norecords')])[{rowIndex}]/td[2]//textarea[@aria-invalid='false']");
        private static By ObstacleType(int rowIndex) => By.XPath($"(.//td[3]//div[@aria-haspopup='listbox'])[{rowIndex}]");
        private static By ObstacleRoam(int rowIndex) => By.XPath($"(.//tr//td[4]//div[@aria-haspopup='listbox'])[{rowIndex}]");
        private static By ObstacleImpact(int rowIndex) => By.XPath($"(.//td[5]//div[@aria-haspopup='listbox'])[{rowIndex}]");
        private static By ObstacleOwners(int rowIndex) => By.XPath($"(.//td[6]//input[contains(@id,'owners-multiple')])[{rowIndex}]");

        private static By ObstacleOwnerName(int rowIndex) => By.XPath($"(.//td[6]//input[contains(@id,'owners-multiple')]/parent::div)[{rowIndex}]");
        private static By ObstacleStatus(int rowIndex) => By.XPath($"(.//td[7]//div[@aria-haspopup='listbox'])[{rowIndex}]");
        private static By ObstacleEtaDate(int rowIndex) => By.XPath($"(.//td[8]//input[@type='tel'])[{rowIndex}]");
        private static By ObstacleDropDownValue(string value) => By.XPath($"(.//ul[@role='combobox' or @role='listbox']//span[text()='{value}'])");
        private static By ObstacleOwnerDropdown(string owner) => By.XPath($"//ul[@role='listbox']//*[text()='{owner}']");


        private static readonly By ObstacleRows = By.XPath("//table[contains(@class,'k-table k-grid-table k-table-md')]/tbody/tr[not(normalize-space(@class)='k-table-row k-grid-norecords')]");

        #endregion

        //Methods

        #region Obstacles

        public void ClickOnAddObstacleButton()
        {
            Log.Step(nameof(ObstaclesTabPage), $"Click on 'Add Obstacle' button");
            Wait.UntilElementClickable(AddNewObstacleButton).Click();
        }

        public bool IsObstacleTitleDisplayed(string obstacleTitle)
        {
            return Driver.IsElementDisplayed(ObstacleTitleFieldValue(obstacleTitle));//
        }
        public void DeleteObstacle(string title)
        {
            Log.Step(nameof(ObstaclesTabPage), $"Click on 'Delete' button for Obstacle");
            ClickOnActionsKebabIcon(title);
            ClickOnActionsDeleteOption();
            ClickOnRemoveButton();
        }

        
        public void AddObstacle(BusinessOutcomeObstaclesRequest obstacleRequest, List<string> owner = null,bool edit = false)
        {
            Log.Step(nameof(ObstaclesTabPage), "Add new Obstacle");

            if (!edit)
            {
                ClickOnAddObstacleButton();
            }
            
            var newRowIndex = Driver.GetElementCount(ObstacleRows);

            // Enter Title//
            Wait.UntilElementVisible(ObstacleTitle(newRowIndex)).ClearTextbox();
            Wait.UntilElementVisible(ObstacleTitle(newRowIndex)).SetText(obstacleRequest.Title);

            // Enter Description
            Wait.UntilElementVisible(ObstacleDescription(newRowIndex)).ClearTextbox();
            Wait.UntilElementVisible(ObstacleDescription(newRowIndex)).SetText(obstacleRequest.Description);
            Wait.HardWait(2000);

            // Select Obstacle Type (Dropdown)
            Wait.UntilElementClickable(ObstacleType(newRowIndex)).Click();
            Wait.HardWait(2000);
            Wait.UntilElementClickable(ObstacleDropDownValue(((BusinessOutcomesObstacles)obstacleRequest.ObstacleType).ToString())).Click();
          

            // Select Obstacle Roam (Dropdown)
            if (obstacleRequest.ObstacleType == 2)
            {
                Wait.UntilElementClickable(ObstacleRoam(newRowIndex)).Click();
                Wait.HardWait(2000);
                if (obstacleRequest.Roam != null)
                    Wait.UntilElementClickable(ObstacleDropDownValue(
                        ((RoamType)obstacleRequest.Roam).ToString())).Click();
            }

            // Select Obstacle Impact (Dropdown)
            Wait.UntilElementClickable(ObstacleImpact(newRowIndex)).Click();
            Wait.HardWait(2000);
            Wait.UntilElementClickable(ObstacleDropDownValue(obstacleRequest.Impact)).Click();
        

            // Select Obstacle Status (Dropdown)
            Wait.UntilElementClickable(ObstacleStatus(newRowIndex)).Click();
            Wait.HardWait(2000);
            Wait.UntilElementClickable(ObstacleDropDownValue(((StatusType)obstacleRequest.Status).GetDescription())).Click();

            // Enter ETA Date
            if (obstacleRequest.EndDate != null)
                Wait.UntilElementVisible(ObstacleEtaDate(newRowIndex))
                    .SendKeys(obstacleRequest.EndDate.Value.ToString("MM/dd/yyyy"));

            SelectOwner(owner);

        }

        public List<BusinessOutcomeObstaclesResponse> GetObstaclesResponse()
        {
            Log.Step(nameof(ObstaclesTabPage), "Get the created Obstacle response");
            var newRowIndex = Driver.GetElementCount(ObstacleRows);
            var list = new List<BusinessOutcomeObstaclesResponse>();
            var obstacleRoamDropdown = "";

            for (var i = 1; i <= newRowIndex; i++)
            {
                //Get Title
                var obstacleTitle = Wait.UntilElementVisible(ObstacleTitle(i)).GetText();

                // Get Description
                var obstacleDescription = Wait.UntilElementVisible(ObstacleDescription(i)).GetText();

                // Select Obstacle Type (Dropdown)
                var obstacleTypeDropdown = Wait.UntilElementVisible(ObstacleType(i)).GetText();

                if (Driver.IsElementDisplayed(ObstacleRoam(i)))
                {
                    obstacleRoamDropdown = Wait.UntilElementVisible(ObstacleRoam(i)).GetText();
                }


                var obstacleImpactDropdown = Wait.UntilElementVisible(ObstacleImpact(i)).GetText();

                var obstacleImpactDropDown = Wait.UntilElementVisible(ObstacleStatus(i)).GetText().Replace(" ", "");
                var obstacleOwner = Wait.UntilElementVisible(ObstacleOwnerName(i)).GetText();

                // Enter ETA Date
                var endDate = Wait.UntilElementVisible(ObstacleEtaDate(i)).GetText();


                var obstacleEndDate = DateTime.TryParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempDate) ? tempDate : (DateTime?)null;

                list.Add(new BusinessOutcomeObstaclesResponse()
                {
                    Title = obstacleTitle,
                    Description = obstacleDescription,
                    ObstacleType = (System.Enum.TryParse(obstacleTypeDropdown, out BusinessOutcomesObstacles obstaclesType)) ? (int)obstaclesType : 0,
                    Roam = (System.Enum.TryParse(obstacleRoamDropdown, out RoamType roam)) ? (int)roam : 0,
                    Impact = obstacleImpactDropdown,
                    Status = (System.Enum.TryParse(obstacleImpactDropDown, out StatusType status)) ? (int)status : 0,
                    ObstacleOwners = new List<BusinessOutcomeObstacleOwnerResponse>()
                  {
                      new BusinessOutcomeObstacleOwnerResponse()
                      {
                          DisplayName = obstacleOwner
                      }
                  },
                    EndDate = obstacleEndDate
                });
            }

            return list;
        }

        public void SelectOwner(List<string> owners, int rowIndex = 0)
        {
            Log.Step(nameof(ObstaclesTabPage), "Click on 'Select Owners'");
            if (rowIndex == 0)
            {
                rowIndex = Driver.GetElementCount(ObstacleRows);
            }
            foreach (var owner in owners)
            {
                Wait.UntilElementExists(ObstacleOwners(rowIndex)).Click();
                var element = Wait.UntilElementExists(ObstacleOwners(rowIndex)).SetText(owner);
                Wait.HardWait(3000);// Wait Added for Owner Dropdown
                element.SetText(owner).Click();
                Wait.HardWait(4000);//Wait Added to search for Owners
                var ownerDropdown = Wait.UntilElementExists(ObstacleOwnerDropdown(owner));
                ownerDropdown.Click();
            }
        }
    }
    #endregion
}


