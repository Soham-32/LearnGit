using System.Collections.Generic;

namespace AtCommon.Dtos.GlobalTranslation
{
    public class GlobalTranslations
    {
        public IList<Languages> Languages { get; set; }
    }

    public class AssessmentPageResponse
    {
        public string Title { get; set; }
    }

    public class CompanyDashboardPageResponse
    {
        public string Title { get; set; }
        public string AddACompanyButton { get; set; }
        public string Companyname { get; set; }
    }

    public class Languages
    {
        public string Language { get; set; }
        public LoginPageResponse LoginPageResponse { get; set; }
        public CompanyDashboardPageResponse CompanyDashboardPageResponse { get; set; }
        public TeamDashboardPageResponse TeamDashboardPageResponse { get; set; }
        public TeamAssessmentDashboardPageResponse TeamAssessmentDashboardPageResponse { get; set; }
        public AssessmentPageResponse AssessmentPageResponse { get; set; }
    }

    public class LoginPageResponse
    {
        public string Title { get; set; }
        public string TitleDescription { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ForgotPassword { get; set; }
        public string KeepMeLoggedIn { get; set; }
        public string DoNotHaveAccount { get; set; }
        public string LoginButton { get; set; }
        public string EmailValidation { get; set; }
        public string PasswordValidation { get; set; }
    }

    public class TeamAssessmentDashboardPageResponse
    {
        public string Title { get; set; }
        public List<string> DashboardList { get; set; }
        public string AddAnAssessmentButton { get; set; }
        public string ThRadar { get; set; }
    }

    public class TeamDashboardPageResponse
    {
        public string AddATeamButton { get; set; }
        public List<string> DashboardList { get; set; }
    }

}