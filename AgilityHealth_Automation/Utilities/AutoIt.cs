using AgilityHealth_Automation.DataObjects;
using System;
using System.IO;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Utilities
{
    internal static class AutoIt
    {

        internal static void AddReviewerOnEditRevieweePopup(Reviewer reviewer)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\AutoIt\Assessments\EditRevieweePopup_AddReviewer.exe");
            var commandLineArguments = $"{reviewer.FirstName} {reviewer.LastName} {reviewer.Email}";
            CSharpHelpers.RunExternalExe(filePath, commandLineArguments);
        }

        internal static void EnterAssessmentNoteDescription(string description)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\AutoIt\Assessments\EditAssessmentNote.exe");
            CSharpHelpers.RunExternalExe(filePath, "\""  + description + "\"");
        }

        internal static void InternetExplorerDownloadClickOnSave(string windowTitle)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\AutoIt\Download\DownloadAndSave.exe");
            windowTitle = windowTitle.Replace(" ", "*"); //Replacing spaces with '*'
            CSharpHelpers.RunExternalExe(filePath, windowTitle);
        }
    }
}
