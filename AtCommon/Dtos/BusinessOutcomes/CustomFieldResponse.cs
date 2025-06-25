using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CustomFieldResponse
    {
        public Guid Uid { get; set; }        

        public string Name { get; set; }       

        public int CompanyId { get; set; }      
        
        public int Order { get; set; }              
    }
}