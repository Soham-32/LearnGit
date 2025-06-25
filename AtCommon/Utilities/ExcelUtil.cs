using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AtCommon.Dtos.Bulk;
using ClosedXML.Excel;
using OfficeOpenXml;


namespace AtCommon.Utilities
{
    public class ExcelUtil
    {
        public static DataTable GetExcelData(string excelPath, string sheetName = "")
        {
            var exlFile = new FileInfo(excelPath);

            var package = new ExcelPackage(exlFile);
            var oSheet = sheetName != ""
                ? package.Workbook.Worksheets[sheetName]
                : package.Workbook.Worksheets.FirstOrDefault().CheckForNull();

            var rowCount = oSheet.Dimension.End.Row;
            var colCount = oSheet.Dimension.End.Column;
            var dt = new DataTable(oSheet.Name);
            DataRow dr = null;
            for (var i = 1; i <= rowCount; i++)
            {
                if (i > 1) dr = dt.Rows.Add();
                for (var j = 1; j <= colCount; j++)
                {
                    if (i == 1)
                        dt.Columns.Add(oSheet.Cells[i, j].Value?.ToString());
                    else
                    {
                        var value = oSheet.Cells[i, j].Value;
                        value = value != null ? value.ToString() : "";

                        if (dr != null) dr[j - 1] = value;
                    }
                }
            }

            return dt;
        }

        public static List<string> GetWorkSheetNames(string excelPath)
        {
            var exlFile = new FileInfo(excelPath);

            var package = new ExcelPackage(exlFile);
            return package.Workbook.Worksheets.Select(w => w.Name).ToList();
        }

        public static void GenerateImportFile(string templatePath, string newFilePath, List<AddTeam> teams, List<AddMembers> members, List<AddUser> users)
        {
            var newFile = new FileInfo(newFilePath);
            using var package = new ExcelPackage(newFile);
            
            
            // add teams
            var teamsTemplate = GetExcelData(templatePath, "Teams");
            var teamsWorksheet = package.Workbook.Worksheets.Add("Teams");
            foreach (var team in teams)
            {
                var newTeamRow = teamsTemplate.NewRow();
                newTeamRow["TeamUid"] = team.ExternalIdentifier;
                newTeamRow["Name"] = team.Name;
                newTeamRow["Description"] = team.Description;
                newTeamRow["Bio"] = team.Bio;
                if (team.AgileAdoptionDate != null)
                    newTeamRow["AgileAdoptionDate"] = team.AgileAdoptionDate.Value.ToString("yyyy-MM-dd");
                newTeamRow["Department"] = team.Department;
                if (team.FormationDate != null)
                    newTeamRow["FormationDate"] = team.FormationDate.Value.ToString("yyyy-MM-dd");
                newTeamRow["Type"] = (team.Type == "Team") ? team.Tags.Single(t => t.Category == "Work Type").Tags.First() : team.Type;
                newTeamRow["Methodology"] = team.Tags.Single(t => t.Category == "Methodology").Tags.First();
                teamsTemplate.Rows.Add(newTeamRow); 
            }
            teamsWorksheet.Cells["A1"].LoadFromDataTable(teamsTemplate, true);

            // add members
            var membersWorksheet = package.Workbook.Worksheets.Add("Members");
            var membersTemplate = GetExcelData(templatePath, "Members");
            foreach (var member in members)
            {
                var newMemberRow = membersTemplate.NewRow();
                newMemberRow["TeamUid"] = member.TeamExternalIdentifier;
                newMemberRow["FirstName"] = member.FirstName;
                newMemberRow["LastName"] = member.LastName;
                newMemberRow["Email"] = member.Email;
                newMemberRow["IsStakeholder"] = (member.IsStakeholder) ? 1 : 0;
                newMemberRow["Role"] = string.Join(",", member.Roles);
                newMemberRow["Participant Group"] = string.Join(",", member.ParticipantGroups);
                membersTemplate.Rows.Add(newMemberRow); 
            }
            membersWorksheet.Cells["A1"].LoadFromDataTable(membersTemplate, true);
            
            // add users
            var usersWorksheet = package.Workbook.Worksheets.Add("Users");
            var usersTemplate = GetExcelData(templatePath, "Users");
            foreach (var user in users)
            {
                var newUserRow = usersTemplate.NewRow();
                newUserRow["FirstName"] = user.FirstName;
                newUserRow["LastName"] = user.LastName;
                newUserRow["Email"] = user.Email;
                newUserRow["Role"] = user.Role;
                newUserRow["Tags"] = $"Business Lines|{SharedConstants.TeamTag}";
                usersTemplate.Rows.Add(newUserRow); 
            }
            usersWorksheet.Cells["A1"].LoadFromDataTable(usersTemplate, true);
            
            package.Save();
        }
       
        public static void CreateExcel(Dictionary<string,double> dictionary)
        {
            // File path for the new Excel file
            var fileName = "PageLoadTime_" + new DateTime().ToString("mmddyyyyhhmmss");
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources\\Logs\\{fileName}.xlsx");

            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }

            // Create a new Excel package
            var excelPackage = new ExcelPackage();

            // Add a worksheet named "PageVsLoadTime"
            var worksheet = excelPackage.Workbook.Worksheets.Add("PageVsLoadTime");

            // Add column headers
            worksheet.Cells[1, 1].Value = "Page Name";
            worksheet.Cells[1, 2].Value = "Load Time (in Secs)";

            // Find the next available row
            var row = 2;

            // Append dictionary data to Excel
            foreach (var item in dictionary)
            {
                worksheet.Cells[row, 1].Value = item.Key;    // Column A for keys
                worksheet.Cells[row, 2].Value = item.Value;  // Column B for values
                row++;
            }

            // Save Excel package to the file
            var excelFile = new FileInfo(filePath);
            excelPackage.SaveAs(excelFile);
        }

        public static void ExcelColumnAutoAdjustExcel(string spreadSheet)
        {
            using (var workbook = new XLWorkbook(spreadSheet))
            {
                var worksheet = workbook.Worksheet(1); // Adjust index or name as needed

                // Auto-fit all columns in the worksheet
                worksheet.Columns().AdjustToContents();

                // Save the modified workbook
                workbook.SaveAs(spreadSheet);
            }
        }
    }
}