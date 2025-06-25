using AtCommon.Api;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Dtos.AhTrial;

namespace ApiTests.v1.Endpoints.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial"), TestCategory("Public")]
    public class AhTrialAddTrialCompanyTests : BaseV1Test
    {
        public static string InvalidStringData = "invalidTestData";

        [TestMethod]
        [TestCategory("KnownDefect")]
        public async Task AhTrial_Post_AddTrialCompany_Success()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //When
            var response = await client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.IsTrue(response.Dto.Id > 0, $"{response.Dto.Id} Id is null or less then zero");
            Assert.AreEqual(ahTrialCompanyRequest.Name, response.Dto.Name, "Company Name does not match");
            Assert.AreEqual(ahTrialCompanyRequest.Industry, response.Dto.Industry, "Industry does not match");
            Assert.AreEqual(ahTrialCompanyRequest.Country, response.Dto.Country, "Country does not match");
            Assert.AreEqual(ahTrialCompanyRequest.FirstName, response.Dto.CompanyFirstName, "FirstName does not match");
            Assert.AreEqual(ahTrialCompanyRequest.LastName, response.Dto.CompanyLastName, "LastName does not match");
            Assert.AreEqual(ahTrialCompanyRequest.Email, response.Dto.CompanyEmail, "Company Email does not match");
            Assert.AreEqual(ahTrialCompanyRequest.Phone, response.Dto.CompanyPhone, "Phone does not match");
            Assert.AreEqual(ahTrialCompanyRequest.Email, response.Dto.CompanyAdmin, "CompanyAdmin does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        public async Task AhTrial_Post_AddTrialCompany_Industry_InvalidValue_Success()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Invalid industry value
            ahTrialCompanyRequest.Industry = InvalidStringData;

            //When
            var response = await client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status codes does not match");
            Assert.AreEqual("Not Set", response.Dto.Industry, "Industry does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_NullValues_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = new AhTrialCompanyRequest()
            {
                Name = null,
                Industry = null,
                Country = null,
                FirstName = null,
                LastName = null,
                Email = null,
                Phone = null,
                Password = null,
                TrialType = null
            };

            var errorResponseList = new List<string>
            {
                "'Name' must not be empty.",
                "'CompanyName' is required",
                "'First Name' must not be empty.",
                "'First Name' is required",
                "'Last Name' must not be empty.",
                "'Last Name' is required",
                "'Country' must not be empty.",
                "'Country' is required",
                "'Industry' must not be empty.",
                "'Industry' is required",
                "'Email' must not be empty.",
                "'Email' is required",
                "'Phone' must not be empty.",
                "'Phone' is required",
                "'Password' must not be empty.",
                "'Password' is required",
                "Your password cannot be empty",
                "'Trial Type' must not be empty.",
                "The specified condition was not met for 'Trial Type'."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_EmptyValues_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = new AhTrialCompanyRequest()
            {
                Name = "",
                Industry = "",
                Country = "",
                FirstName = "",
                LastName = "",
                Email = "",
                Phone = "",
                Password = "",
                TrialType = ""
            };
            var errorResponseList = new List<string>
            {
                "'Name' must not be empty.",
                "'First Name' must not be empty.",
                "'Last Name' must not be empty.",
                "'Country' must not be empty.",
                "'Industry' must not be empty.",
                "'Email' must not be empty.",
                "'Email' should be in a valid format.",
                "'Phone' must not be empty.",
                "Phone Number is not valid.",
                "'Password' must not be empty.",
                "Your password cannot be empty",
                "Your password length must be at least 8.",
                "Your password must contain at least one uppercase letter.",
                "Your password must contain at least one lowercase letter.",
                "Your password must contain at least one number.",
                "Your password must contain at least one special character (!? *.@#$&).",
                "'Trial Type' must not be empty.",
                "The specified condition was not met for 'Trial Type'."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        public async Task AhTrial_Post_AddTrialCompany_CompanyName_Existing_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            var response = await client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);
            response.EnsureSuccess();

            //When
            var responseWithExistingCompanyName = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, responseWithExistingCompanyName.StatusCode, "The status codes does not match");
            Assert.AreEqual("Company already exists.", responseWithExistingCompanyName.Dto.ListToString(), "Error message does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_Email_InvalidValue_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Invalid email format
            ahTrialCompanyRequest.Email = InvalidStringData;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.AreEqual("'Email' should be in a valid format.", response.Dto.ListToString(), "Error message does not matched");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        public async Task AhTrial_Post_AddTrialCompany_Email_Existing_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();
            
            var response = await client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);
            response.EnsureSuccess();

            //Update the 'Company Email' with existing company email
            var ahTrialCompanyRequestWithExistingEmail = AhTrialFactory.GetValidAhTrialCompany();
            ahTrialCompanyRequestWithExistingEmail.Email = ahTrialCompanyRequest.Email;

            //When
            var responseWithExistingCompanyEmail = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequestWithExistingEmail);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, responseWithExistingCompanyEmail.StatusCode, "The status codes does not match");
            Assert.AreEqual("User already exists.", responseWithExistingCompanyEmail.Dto.ListToString(), "Error message does not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        public async Task AhTrial_Post_AddTrialCompany_PhoneNumber_Existing_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            var response = await client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);
            response.EnsureSuccess();

            //Update the 'Phone Number' with existing phone number
            var ahTrialCompanyRequestWithExistingPhone = AhTrialFactory.GetValidAhTrialCompany();
            ahTrialCompanyRequestWithExistingPhone.Phone = ahTrialCompanyRequest.Phone;

            //When
            var responseWithExistingPhone = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequestWithExistingPhone);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, responseWithExistingPhone.StatusCode, "The status codes does not match");
            Assert.AreEqual("Phone Number already exists.", responseWithExistingPhone.Dto.ListToString(), "Error message does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_PhoneNumber_InvalidValue_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Invalid phone number 
            ahTrialCompanyRequest.Phone = InvalidStringData;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.AreEqual("Phone Number is not valid.", response.Dto.ListToString(), "Error message does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_PhoneNumber_MoreThanMaximumLength_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Phone number with maximum length value
            ahTrialCompanyRequest.Phone = CSharpHelpers.RandomString(20);
            var errorResponseList = new List<string>
            {
                $"The length of 'Phone' must be 18 characters or fewer. You entered {ahTrialCompanyRequest.Phone.Length} characters.",
                "Phone Number is not valid."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_Password_WithoutNumericValue_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Password without numeric value
            ahTrialCompanyRequest.Password = "Demo@dsjfla";

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.AreEqual("Your password must contain at least one number.", response.Dto.ListToString(), "Error message does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_Password_MoreThanMaximumLength_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Password with more than maximum length
            ahTrialCompanyRequest.Password = "passwordTest$$&@@123456";

            var errorResponseListOfPasswordLength = new List<string>
            {
                $"The length of 'Password' must be 20 characters or fewer. You entered {ahTrialCompanyRequest.Password.Length} characters.",
                "Your password length must not exceed 16."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseListOfPasswordLength, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_Password_NumericValueOnly_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Password with numeric value only
            ahTrialCompanyRequest.Password = CSharpHelpers.Random8Number().ToString();

            var errorResponseListOfInvalidPassword = new List<string>
            {
                "Your password must contain at least one uppercase letter.",
                "Your password must contain at least one lowercase letter.",
                "Your password must contain at least one special character (!? *.@#$&)."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.That.ListsAreEqual(errorResponseListOfInvalidPassword, response.Dto.ToList(), "Error list does not match");
        }

        [TestMethod]
        public async Task AhTrial_Post_AddTrialCompany_TrialType_InvalidValue_BadRequest()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();

            //Invalid TrialType value
            ahTrialCompanyRequest.TrialType = InvalidStringData;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "The status codes does not match");
            Assert.AreEqual("The specified condition was not met for 'Trial Type'.", response.Dto.ListToString(), "Error message does not show");
        }
    }
}
