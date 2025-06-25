
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal
{
    public class GrowthPortalPage : BasePage
    {
        public GrowthPortalPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By PageTitleText = By.XPath("//div[@class = 'container-fluid']//h1");
        private readonly By EditPortalButton = By.ClassName("filter-edit-button");
        private readonly By PublishButton = By.Id("btnPublish");
        private readonly By SaveDraftButton = By.Id("btnSaveDraft");
        private readonly By DeleteDraftButton = By.Id("btnDeleteDraft");
        private readonly By AddResourcesButton = By.Id("addResources");
        private readonly By ResourceTitleTextbox = By.CssSelector("#resourceForm input[name='Title']");
        private readonly By ResourceLinkTextbox = By.CssSelector("#resourceForm input[name='HyperLink']");
        private readonly By FileInput = By.Id("file");
        private readonly By ThumbnailInput = By.Id("imagefile");
        private readonly By ThumbnailNameInput = By.Id("ThumbnailName");
        private readonly By AssessmentDropdown = By.CssSelector("span[aria-owns='surveys-dropdown_listbox']");
        private readonly By SelectAssessmentButton = By.CssSelector(".apply-filter-btn"); 
        private readonly By ResourceCompetencyDiv = By.XPath(
            "//div[@id='resourceForm']//select[@data-placeholder='Type a competency']/preceding-sibling::div");

        private readonly By ResourceCompetencyTextbox = By.XPath(
            "//div[@id='resourceForm']//select[@data-placeholder='Type a competency']/preceding-sibling::div/input");

        private readonly By ResourceDescriptionBody = By.CssSelector("body.k-state-active");
        private readonly By SaveResourceButton = By.Id("btnAddResource");
        private readonly By ResourceDescriptionTextbox = By.CssSelector("#resourceForm .k-editable-area");
        private readonly By ResourceDescriptionIframe = By.CssSelector("#resourceForm iframe");
        private readonly By AcceptDeleteButton = By.CssSelector("#deleteResourceDialog button .k-update");
        private readonly By SaveSuccessMessage = By.ClassName("k-notification-wrap");
        private readonly By StatusBoxImg = By.Id("statusBox");

        private readonly By FileUploadCompleteIndicator = By.XPath(
            "//input[@id='file']/../following-sibling::strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");

        private readonly By ImageUploadCompleteIndicator = By.XPath(
            "//input[@id='imagefile']/../following-sibling::strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private static By AssessmentListItem(string radar) => By.XPath($"//ul[@id='surveys-dropdown_listbox']/li[text()='{radar}']");
        //Key customer verification
        #region Assessment first item
        private readonly By AssessmentFirstItem = By.XPath("//ul[@id='surveys-dropdown_listbox']/li[1]");
        #endregion
        //Left tree view
        private static By TreeViewExpandNodeLocator(string nodeName) => By.XPath(
            $"//span[contains(text(),'{nodeName}')]/ancestor::div/span[contains(@class,'k-plus')]");

        //Key customer verification
        #region First node and sub node tree view
        private readonly By TreeViewExpandFirstNodeLocator = By.XPath("//div[contains(@class,'dimensions-box')]/div[1]/ul/li/div/span[1]");
        private readonly By TreeViewExpandFirstSubNodeLocator = By.XPath("//div[contains(@class,'dimensions-box')]/div[1]/ul/li/ul/li/div/span[1]");
        #endregion

        private static By CompetencyItem(string item) => By.XPath(
            $"//div[contains(@class, 'dimensions-box')]//a[contains(@class, 'treeview-item')][normalize-space(text())='{item}']");

        //Key customer verification
        #region First competency item
        private readonly By CompetencyFirstItem = By.XPath("//div[contains(@class,'dimensions-box')]/div[1]/ul/li/ul/li/ul/li[1]//a");
        #endregion

        private static By CompetencyDropdownItem(string item) => By.XPath(
            $"//ul[@aria-hidden='false' and @data-role='staticlist']/li[text()='{item}']");

        private static By ResourceTitle(string item) => By.XPath(
            $"//div[@id='resourceCarousel']//div[@class='resources-title']/h4[text()='{item}']");

        private static By ResourceThumbnail(string item) => By.XPath(
            $"//div[@id='resourceCarousel']//div[@class='resources-title']/h4[text()='{item}']/../following-sibling::div/div/div[contains(@class, 'resource-thumbnail')]//a/img");

        private static By ResourceLearnMoreLink(string item) => By.XPath(
            $"//div[@id='resourceCarousel']//div[@class='resources-title']/h4[text()='{item}']/../following-sibling::div//div[contains(@class, 'links-wrapper')]//a[@class='pink learn-more']");

        
        private static By ResourceDescription(string item) => By.XPath(
            $"//div[@id='resourceCarousel']//div[@class='resources-title']/h4[text()='{item}']/../following-sibling::div/div/div[contains(@class, 'resource-text')]");

        private static By ResourceFile(string item) => By.XPath(
            $"//div[@id='resourceCarousel']//div[@class='resources-title']/h4[text()='{item}']/../following-sibling::div//div[contains(@class, 'links-wrapper')]//a[@class='pink']");

        private readonly By DeleteResourceButton = By.CssSelector("input[value = 'Delete']");
        // competency sections
        private readonly By HealthDiv = By.CssSelector(".row.health-section");
        private readonly By RecommendationsDiv = By.CssSelector(".row.recommendation-section");
        private readonly By VideosDiv = By.CssSelector(".row.video-section");
        private readonly By ResourcesDiv = By.CssSelector(".row.resources-section");
        private readonly By CoachingDiv = By.CssSelector(".row.coaching-section");


        //Left tree view
        public void TreeViewExpandNode(string nodeName = null)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                // Static case: select the first node - Key customer verification
                Log.Step(nameof(GrowthPortalPage), $"Expand First node");
                Wait.UntilJavaScriptReady();

                if (Driver.IsElementDisplayed(TreeViewExpandFirstNodeLocator))
                {
                    Wait.UntilElementClickable(TreeViewExpandFirstNodeLocator).Click();
                }
            }
            else
            {
                // Dynamic case: select the node
                Log.Step(nameof(GrowthPortalPage), $"Expand node <{nodeName}>");
                Wait.UntilJavaScriptReady();

                if (Driver.IsElementDisplayed(TreeViewExpandNodeLocator(nodeName)))
                {
                    Wait.UntilElementClickable(TreeViewExpandNodeLocator(nodeName)).Click();
                }
            }

        }

        public void TreeViewExpandFirstSubNode()
        {
            Log.Step(nameof(GrowthPortalPage), $"Expand First sub node");
            Wait.UntilJavaScriptReady();

            if (Driver.IsElementDisplayed(TreeViewExpandFirstSubNodeLocator))
            {
                Wait.UntilElementClickable(TreeViewExpandFirstSubNodeLocator).Click();
            }
        }

        public void ClickCompetency(string competency = null)
        {
            if (string.IsNullOrEmpty(competency))
            {
                // Static case: select the first competency - Key customer verification
                Log.Step(nameof(GrowthPortalPage), $"Click on first competency");
                Wait.UntilElementClickable(CompetencyFirstItem).Click();
            }
            else
            {
                // Dynamic case: select the competency
                Log.Step(nameof(GrowthPortalPage), $"Click on competency <{competency}>");
                Wait.UntilElementClickable(CompetencyItem(competency)).Click();
            }
        }

        public void ClickEditPortalButton()
        {
            Log.Step(nameof(GrowthPortalPage), "Click on Edit Portal button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(EditPortalButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickPublishButton()
        {
            Log.Step(nameof(GrowthPortalPage), "Click on Publish button");
            Wait.UntilElementClickable(PublishButton).Click();
            Wait.UntilElementInvisible(StatusBoxImg);
        }

        public void ClickSaveDraftButton()
        {
            Wait.UntilElementClickable(SaveDraftButton).Click();
            Wait.UntilElementVisible(SaveSuccessMessage);
        }

        public void ClickDeleteDraftButton()
        {
            Wait.UntilElementClickable(DeleteDraftButton).Click();
            Wait.UntilElementInvisible(StatusBoxImg);
        }

        public void ClickAddResourceButton()
        {
            Log.Step(nameof(GrowthPortalPage), "Click on Add Resource button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddResourcesButton).Click();
        }

        public void EnterResourceTitle(string title)
        {
            Log.Step(nameof(GrowthPortalPage), $"Enter resource title <{title}>");
            Wait.UntilElementVisible(ResourceTitleTextbox).SetText(title);
        }

        public void EnterResourceLink(string link)
        {
            Log.Step(nameof(GrowthPortalPage), $"Enter resource link <{link}>");
            Wait.UntilElementVisible(ResourceLinkTextbox).SetText(link);
        }

        public void EnterDescription(string description)
        {
            Wait.UntilElementClickable(ResourceDescriptionTextbox).Click();
            Driver.SwitchToFrame(ResourceDescriptionIframe);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ResourceDescriptionBody).SetText(description);
            Wait.UntilJavaScriptReady();
            Driver.SwitchTo().DefaultContent();
        }

        public void SelectResourceCompetency(string competency)
        {
            Log.Step(nameof(GrowthPortalPage), $"Select resource competency <{competency}>");
            Wait.UntilElementClickable(ResourceCompetencyDiv).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(ResourceCompetencyTextbox).SetText(competency);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(CompetencyDropdownItem(competency)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void UploadResourceFile(string file)
        {
            Log.Step(nameof(GrowthPortalPage), $"Upload resource file <{file}>");
            Wait.UntilElementExists(FileInput).SetText(file, false);
            Wait.UntilElementExists(FileUploadCompleteIndicator);
        }

        public void UploadResourceThumbnail(string file)
        {
            Log.Step(nameof(GrowthPortalPage), $"Upload resource thumbnail <{file}>");
            Wait.UntilElementExists(ThumbnailInput).SetText(file, false);
            Wait.UntilElementExists(ImageUploadCompleteIndicator);
        }

        public string GetThumbnail() => Wait.UntilElementExists(ThumbnailNameInput).GetElementAttribute("value");

        public void ClickSaveResourceButton()
        {
            Log.Step(nameof(GrowthPortalPage), "Click on Save Resource button");
            Wait.UntilElementClickable(SaveResourceButton).Click();
            Wait.UntilElementInvisible(SaveResourceButton);
        }

        public bool DoesResourceDisplay(string title) => Driver.IsElementDisplayed(ResourceTitle(title));

        public bool DoesResourceThumbnailDisplay(string title) => 
            Driver.IsElementDisplayed(ResourceThumbnail(title));

        public bool DoesResourceLinkDisplay(string title, string link) => 
            Wait.UntilElementVisible(ResourceLearnMoreLink(title)).GetElementAttribute("href").Contains(link);

        public bool DoesResourceDescriptionDisplay(string title, string description) => 
            Wait.UntilElementVisible(ResourceDescription(title)).GetText().Contains(description);

        public bool DoesResourceFileDisplay(string title, string fileName) => 
            Wait.UntilElementVisible(ResourceFile(title)).GetElementAttribute("href").Contains(fileName);

        public void DeleteAllResources()
        {
            Log.Step(nameof(GrowthPortalPage), "Delete all resources");
            var buttons = Wait.UntilAllElementsLocated(DeleteResourceButton);
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[0].Click();
                Wait.UntilJavaScriptReady();
                ClickAcceptDeleteResource();
                Wait.UntilJavaScriptReady();
                buttons = Wait.UntilAllElementsLocated(DeleteResourceButton);
            }
        }

        public void ClickResourceLink(string title)
        {
            Log.Step(nameof(GrowthPortalPage), $"Click on resource link <{title}>");
            Wait.UntilElementClickable(ResourceLearnMoreLink(title)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickResourceThumbnail(string title)
        {
            Log.Step(nameof(GrowthPortalPage), $"Click on resource thumbnail <{title}>");
            Wait.UntilElementClickable(ResourceThumbnail(title)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickAcceptDeleteResource()
        {
            Wait.UntilElementClickable(AcceptDeleteButton).Click();
            Wait.UntilElementInvisible(AcceptDeleteButton);
        }

        public void ClickResourceFile(string title)
        {
            Log.Step(nameof(GrowthPortalPage), $"Click on resource file <{title}>");
            Wait.UntilElementClickable(ResourceFile(title)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesSectionDisplay(CompetencySection section)
        {
            return Driver.IsElementDisplayed( section switch
            {
                CompetencySection.Health => HealthDiv,
                CompetencySection.Recommendations => RecommendationsDiv,
                CompetencySection.Videos => VideosDiv,
                CompetencySection.Resources => ResourcesDiv,
                CompetencySection.Coaching => CoachingDiv,
                _ => null
            });
        }

        public bool DoesEditButtonDisplay() => Driver.IsElementDisplayed(EditPortalButton);

        public void SelectAssessment(string assessment = null)
        {
            if (string.IsNullOrEmpty(assessment))
            {
                // Static case: select the first assessment - Key customer verification
                Log.Step(nameof(GrowthPortalPage), $"Select First assessment");
                SelectItem(AssessmentDropdown, AssessmentFirstItem);
            }
            else
            {
                // Dynamic case: select the specific assessment
                Log.Step(nameof(GrowthPortalPage), $"Select assessment <{assessment}>");
                SelectItem(AssessmentDropdown, AssessmentListItem(assessment));
            }
        }

        public void ClickSelectButton()
        {
            Log.Step(nameof(GrowthPortalPage), $"Click Select button");
            Wait.UntilElementClickable(SelectAssessmentButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsPageTitleDisplayed()
        {
            return Driver.IsElementDisplayed(PageTitleText);
        }
        public string GetPageUrl(int companyId ,bool isV2Page = true, bool isNonSaPaUser = true)
        {
            var url = $"{BaseTest.ApplicationUrl}/growthportal";
            if (isV2Page)
            {
                return url;
            }
            return isNonSaPaUser ? url + $"?companyId={companyId}" : url + "?companyId=0";
        }

        //Navigation
        public void NavigateToGrowthPortalPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/growthportal?companyId={companyId}");
        }
    }

    public enum CompetencySection
    {
        Health,
        Recommendations,
        Videos,
        Resources,
        Coaching
    }
}