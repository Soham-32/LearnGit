using System;
using System.IO;
using System.Linq;
using AtCommon.Dtos;
using Newtonsoft.Json.Linq;

namespace AtCommon.Utilities
{
    public class UserConfig
    {
        public UserConfig(string userCode, bool newNav = false)
        {
            var path = newNav ? "/Users/NewNavigation" : "/Users";

            var userConfigFile = userCode switch
            {
                "TA" => $"{path}/teamAdmin.json",
                "TA2" => $"{path}/TeamAdmin2.json",
                "CA" => $"{path}/companyAdmin.json",
                "BL" => $"{path}/businessLineAdmin.json",
                "OL" => $"{path}/orgLeader.json",
                "SA" => $"{path}/siteAdmin.json",
                "M" => $"{path}/member.json",
                "PA" => $"{path}/partnerAdmin.json",
                "SA2" => $"{path}/siteAdmin2.json",
                "MUP" => $"{path}/manageUserPermission.json",
                "CA2" => $"{path}/companyAdmin2.json",
                _ => throw new Exception($"userCode value <{userCode}> is not valid")
            };

            var filePath = AppDomain.CurrentDomain.BaseDirectory + userConfigFile;
            var text = File.ReadAllText(filePath);
            Json = JObject.Parse(text);
        }

        public JObject Json { get; set; }

        public User GetUserByDescription(string description)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var jsonUser = Json["users"]
                .FirstOrDefault(user => user["description"].ToString() == description)
                .CheckForNull($"No user found in the file with description <{description}>");
            return new User(jsonUser["username"]?.ToString(), jsonUser["password"]?.ToString())
            {
                NewPassword = jsonUser["newPassword"]?.ToString(),
                FirstName = jsonUser["firstName"]?.ToString(),
                LastName = jsonUser["lastName"]?.ToString(),
                Description = jsonUser["description"]?.ToString(),
                CompanyName = jsonUser["companyName"]?.ToString(),
                // ReSharper disable once PossibleNullReferenceException
                Type = jsonUser["type"].ToObject<UserType>()
            };
        }

        public Company GetCompanyByEnvironment(string environment)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if(Json["companies"].Any(company => company["environment"].ToString() == environment))
            {
                return Json["companies"].First(company => company["environment"].ToString() == environment).ToObject<Company>();
            }

            throw new NullReferenceException("Company Id is null for environment : " + environment  + ". Aborting test run.");
        }

    }
}