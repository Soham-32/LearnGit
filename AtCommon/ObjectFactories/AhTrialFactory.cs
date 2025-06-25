using AtCommon.Dtos.AhTrial.Custom;
using AtCommon.Utilities;
using System;
using AtCommon.Dtos.AhTrial;

namespace AtCommon.ObjectFactories
{
    public static class AhTrialFactory
    {
        public static AhTrialSignUpForm GetValidAhTrialSignUpFormInfo() => new AhTrialSignUpForm
        {
            FirstName = $"{RandomDataUtil.GetFirstName()}",
            LastName = SharedConstants.TeamMemberLastName,
            Email = $"ah_automation+Trial{Guid.NewGuid():D}@agiletransformation.com",
            CompanyName = $"ZZZ_Trial{RandomDataUtil.GetCompanyName()}",
            PhoneNumber = $"{CSharpHelpers.Random8Number()}",
            Country = "India",
            Industry = "Retail",
            Password = "Test@123",
            ReEnterPassword = "Test@123"
        };

        public static AhTrialCompanyRequest GetValidAhTrialCompany(string trialType = "Enterprise")
        {
            var guid = Guid.NewGuid();
            return new AhTrialCompanyRequest
            {
                Name = $"ZZZ_Trial{RandomDataUtil.GetCompanyName()}",
                Industry = "Consulting",
                Country = "Afghanistan",
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+Trial{guid:D}@agiletransformation.com",
                Phone = $"{CSharpHelpers.Random8Number()}",
                Password = "Demo@123",
                TrialType = trialType
            };
        }
    }

}