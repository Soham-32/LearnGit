using AtCommon.Dtos;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AtCommon.Api;

namespace AtCommon.Utilities
{
    public class DataBaseUtil
    {
        public static void CleanCompanyForAutomation(int companyId, string databaseConnection)
        {
            using var conn = new SqlConnection {ConnectionString = databaseConnection};
            conn.Open();

            var command = new SqlCommand
            {
                CommandText = "CleanCompanyForAutomation",
                CommandType = CommandType.StoredProcedure,
                Connection = conn
            };
            command.Parameters.Add("@companyid", SqlDbType.Int).Value = companyId;
            command.ExecuteNonQuery();
            conn.Close();
            command.Dispose();
            conn.Dispose();
        }

        public static void PopulateCompanyTeamHierarchy(string databaseConnection)
        {
            using var conn = new SqlConnection { ConnectionString = databaseConnection };
            conn.Open();

            var command = new SqlCommand
            {
                CommandText = "PopulateCompanyTeamHierarchy",
                CommandType = CommandType.StoredProcedure,
                Connection = conn
            };
            command.ExecuteNonQuery();
            conn.Close();
            command.Dispose();
            conn.Dispose();
        }

        public static void DeleteTestCompanies(string databaseConnection)
        {
            using var conn = new SqlConnection {ConnectionString = databaseConnection};
            conn.Open();

            var command = new SqlCommand
            {
                CommandText = "__DeleteTestCompany",
                CommandType = CommandType.StoredProcedure,
                Connection = conn
            };

            command.Parameters.AddWithValue("@ByPassDB_QA_NameCheck", 1);
            
            command.ExecuteNonQuery();
            conn.Close();
            command.Dispose();
            conn.Dispose();
        }

        public static string GetDatabaseConnectionString(string env, string password)
        {
            var connectionString = env.ToLower() switch
            {
                "dev" => DevDatabaseConnectionString,
                "qa" => QaDatabaseConnectionString,
                "atqa" => AtQaDatabaseConnectionString,
                "preprod" => PreprodDatabaseConnectionString,
                "demo" => DemoDatabaseConnectionString,
                "load" => LoadDatabaseConnectionString,
                "uat" => UatDatabaseConnectionString,
                _ => ""
            };

            return string.Format(connectionString, password);
        }

        public static readonly string DevDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthDev; UID=automationuser@agilityhealthradar.com; PWD={0};";

        public static readonly string QaDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthQA;  UID=automationuser@agilityhealthradar.com; PWD={0}";

        public static readonly string AtQaDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthATQA;  UID=automationuser@agilityhealthradar.com; PWD={0};";

        public static readonly string PreprodDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthPreprod;  UID=automationuser@agilityhealthradar.com; PWD={0}";

        public static readonly string UatDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthUAT;  UID=automationuser@agilityhealthradar.com; PWD={0}";

        public static readonly string DemoDatabaseConnectionString =
            "Data Source=t0xz67qtcg.database.windows.net; Authentication=Active Directory Password; Initial Catalog=AgilityHealthDemo;  UID=automationuser@agilityhealthradar.com; PWD={0}";

        public static readonly string LoadDatabaseConnectionString =
            "Data Source=sqls-ahboa-cus-load.database.windows.net,1433; Authentication=Active Directory Password; Initial Catalog=AgilityHealthLoad; UID=automationuser@agilityhealthradar.com; PWD={0};";

        public static string GetProductionDatabaseConnectionString(string clientName,string password)
        {
            string ProdDatabaseConnectionString(string dbname) =>
                $"Data Source=jip0zqd507.database.windows.net; Authentication=Active Directory Password; Initial Catalog={dbname}; UID=automationuser@agilityhealthradar.com; PWD={{0}}";

            var jsonConfig = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentDatabase.json").DeserializeJsonObject<ProductionEnvironmentDatabase>();

            var databaseName = jsonConfig.Environments.Where(a => a.Companyname.Equals(clientName)).Select(a => a.DatabaseName).ToList().FirstOrDefault();

            var connectionString = ProdDatabaseConnectionString(databaseName);

            return string.Format(connectionString,password);
        }
    }


}
