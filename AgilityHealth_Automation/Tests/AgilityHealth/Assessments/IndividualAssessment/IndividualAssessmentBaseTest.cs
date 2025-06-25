using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment
{
    public class IndividualAssessmentBaseTest : BaseTest
    {
        private static MemberResponse _member;
        
        public static TeamResponse GetOrgTeamForIndividualAssessment(SetupTeardownApi setup, int numberOfNewMembers = 1)
        {
            var email = TestEnvironment.UserConfig.GetUserByDescription("member").Username;

            var team = setup.GetTeamResponse(Constants.GoiTeam);
            _member = team.Members.First(m => m.Email == email).CheckForNull();
            
            if (numberOfNewMembers == 1)
            {
                team.Members.Clear();
                team.Members.Add(_member);
            }
            else
            {
                team.Members.RemoveAll(m => m.Email == email);
            }
            return team;
        }
        
        public static TeamResponse GetTeamForIdeaboard(int noOfNewMembers = 1)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            TeamResponse team;
                
            if (User.IsOrganizationalLeader())
            {
                team = GetOrgTeamForIndividualAssessment(setup, noOfNewMembers);
            }
            else
            {
                var teamRequest = TeamFactory.GetGoiTeam("IdeaboardTeam", noOfNewMembers);
                team = setup.CreateTeam(teamRequest).GetAwaiter().GetResult();
            }

            return team;
        }

        public static TeamResponse GetTeamForIndividualAssessment(SetupTeardownApi setup, string teamName, int noOfNewMembers = 1, User admin = null, User member = null)
        {
            //var setup = new SetupTeardownApi(TestEnvironment);
            TeamResponse team;
                
            if (User.IsOrganizationalLeader())
            {
                team = GetOrgTeamForIndividualAssessment(setup, noOfNewMembers);
            }
            else
            {
                var teamRequest = TeamFactory.GetGoiTeam(teamName, noOfNewMembers);

                if (member != null)
                {
                    teamRequest.Members.Add(member.ToAddMemberRequest());
                }
                
                if (User.IsMember()) teamRequest.Members = new List<AddMemberRequest> { User.ToAddMemberRequest() };

                team = setup.CreateTeam(teamRequest, admin).GetAwaiter().GetResult();
            }

            return team;
        }
        
        public static TeamResponse GetTeamForBatchEdit(SetupTeardownApi setup, string teamName, int noOfNewMembers = 1, User member = null)
        {
            //var setup = new SetupTeardownApi(TestEnvironment);
            TeamResponse team;
                
            if (User.IsOrganizationalLeader())
            {
                team = GetOrgTeamForIndividualAssessment(setup, noOfNewMembers);
            }
            else
            {
                var teamRequest = TeamFactory.GetGoiTeam(teamName, noOfNewMembers);

                if (member != null)
                {
                    teamRequest.Members.Add(member.ToAddMemberRequest());
                }
                if (User.IsMember()) teamRequest.Members = new List<AddMemberRequest> { User.ToAddMemberRequest() };
                team = setup.CreateTeam(teamRequest).GetAwaiter().GetResult();
            }

            return team;
        }
        public static Tuple<RadarDetailResponse, CreateIndividualAssessmentRequest, IndividualAssessmentResponse> GetIndividualAssessment(SetupTeardownApi setup, TeamResponse team, string assessmentName, User user = null)
        {
            var radarResponse = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);
            var assessmentResponse = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                Company.Id, User.CompanyName, team.Uid, $"{assessmentName}{Guid.NewGuid()}");
            assessmentResponse.Members = team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            var assessment = setup.CreateIndividualAssessment(assessmentResponse, SharedConstants.IndividualAssessmentType, user)
                .GetAwaiter().GetResult();

            return Tuple.Create(radarResponse, assessmentResponse, assessment);
        }
    }
}