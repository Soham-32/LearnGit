using AtCommon.Utilities;
using System;
using System.Data;
using System.IO;
using System.Linq;

public static class RandomDataUtil
{
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\Test_Dataset.xlsx");
    private static readonly DataTable MasterData = ExcelUtil.GetExcelData(FilePath, "Master");

    private static readonly Random Random = new Random();

    private static string GetRandomValueFromColumn(string columnName)
    {
        var values = MasterData.AsEnumerable()
                               .Select(row => row[columnName].ToString())
                               .Where(val => !string.IsNullOrWhiteSpace(val))
                               .ToList();

        return values.Count > 0 ? values[Random.Next(values.Count)] : string.Empty;
    }

    public static string GetFirstName() => GetRandomValueFromColumn("FirstName");
    public static string GetLastName() => GetRandomValueFromColumn("LastName");
    public static string GetUserName() => GetRandomValueFromColumn("UserName");
    public static string GetEmail() => GetRandomValueFromColumn("Email");
    public static string GetNormalizedEmail() => GetRandomValueFromColumn("NormalizedEmail");
    public static string GetNormalizedUserName() => GetRandomValueFromColumn("NormalizedUserName");
    public static string GetBio() => GetRandomValueFromColumn("Bio");
    public static string GetCompanyName() => GetRandomValueFromColumn("Company_Name");
    public static string GetCompanyCountry() => GetRandomValueFromColumn("Company_Country");
    public static string GetCompanyCity() => GetRandomValueFromColumn("Company_City");
    public static string GetCompanyState() => GetRandomValueFromColumn("Company_State");
    public static string GetCompanyZipCode() => GetRandomValueFromColumn("Company_ZipCode");
    public static string GetTeamName() => GetRandomValueFromColumn("Team_Name");
    public static string GetTeamDescription() => GetRandomValueFromColumn("Team_Description");
    public static string GetTeamBio() => GetRandomValueFromColumn("Team_Bio");
    public static string GetTeamDepartment() => GetRandomValueFromColumn("Team_Department");
    public static string GetGrowthPlanTitle() => GetRandomValueFromColumn("Growth Plan Title");
    public static string GetGrowthPlanDescription() => GetRandomValueFromColumn("Description");
    public static string GetGrowthPlanAddedBy() => GetRandomValueFromColumn("GrowthPlan_AddedBy");
    public static string GetGrowthPlanOwner() => GetRandomValueFromColumn("GrowthPlan_Owner");
    public static string GetGrowthPlanUserCreatedBy() => GetRandomValueFromColumn("GrowthPlan_UserCreatedBy");
    public static string GetGrowthPlanUserUpdatedBy() => GetRandomValueFromColumn("GrowthPlan_UserUpdatedBy");
    public static string GetAssessmentName() => GetRandomValueFromColumn("Assessment_Name");
    public static string GetFacilitator() => GetRandomValueFromColumn("Facilitator");
    public static string GetFacilitatorEmail() => GetRandomValueFromColumn("FacilitatorEmail");
    public static string GetIndividualAssessmentPOC() => GetRandomValueFromColumn("IndividualAssessment_PointOfContact");
    public static string GetIndividualAssessmentPOCEmail() => GetRandomValueFromColumn("IndividualAssessment_PointOfContactEmail");
    public static string GetSubdimensionNote() => GetRandomValueFromColumn("SubdimensionNote");
    public static string GetDimensionNote() => GetRandomValueFromColumn("DimensionNote");
    public static string GetGrowthPlanComment() => GetRandomValueFromColumn("GrowthPlan_Comment");
    public static string GetPulseAssessmentName() => GetRandomValueFromColumn("PulseAssessment_Name");
    public static string GetBusinessOutcomeOwner() => GetRandomValueFromColumn("BusinessOutcome_Owner");
    public static string GetBusinessOutcomeTitle() => GetRandomValueFromColumn("Business Outcome Title");
    public static string GetBusinessOutcomeDeletedBy() => GetRandomValueFromColumn("BusinessOutcome_DeletedBy");
    public static string GetBusinessOutcomeSourceCategoryName() => GetRandomValueFromColumn("BusinessOutcome_SourceCategoryName");
    public static string GetBusinessOutcomeDescription() => GetRandomValueFromColumn("Business Outcome Description");
    public static string GetKeyResultsName() => GetRandomValueFromColumn("Key Results Name");
    public static string GetChecklistItem() => GetRandomValueFromColumn("Checklist Item ");
}
