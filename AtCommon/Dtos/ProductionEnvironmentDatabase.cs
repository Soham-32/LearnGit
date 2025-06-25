using System.Collections.Generic;

namespace AtCommon.Dtos
{

    public class Environments
    {
        public string Name { get; set; }
        public string Companyname { get; set; }
        public int CompanyId { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseConnectionString { get; set; }
    }

    public class ProductionEnvironmentDatabase
    {
        public List<Environments> Environments { get; set; }
    }
}