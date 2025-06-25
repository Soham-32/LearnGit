using System.Linq;
using AtCommon.Utilities;
using AtCommon.Dtos;

namespace AgilityHealth_Automation.ObjectFactories
{
    public class ProductionEnvironmentDatabaseFactory
    {
        public static Environments GetProductionDatabaseConnectionInfo(ProductionEnvironmentDatabase productionEnvironmentTestData, string companyName, string databasePassword = null)
        {
            return new Environments
            {
                DatabaseConnectionString = DataBaseUtil.GetProductionDatabaseConnectionString(companyName, databasePassword),
                CompanyId = productionEnvironmentTestData.Environments.Where(a => a.Companyname.Equals(companyName))
                    .Select(a => a.CompanyId).ToList().FirstOrDefault(),
                Name = productionEnvironmentTestData.Environments.Where(a => a.Companyname.Equals(companyName))
                    .Select(a => a.Name).ToList().FirstOrDefault()
            };
        }
    }
}