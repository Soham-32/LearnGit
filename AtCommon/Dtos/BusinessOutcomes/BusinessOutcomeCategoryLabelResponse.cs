using System;
using System.Collections.Generic;
using System.Linq;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeCategoryLabelResponse
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool KanbanMode { get; set; }

        public int? BusinessOutcomeCardTypeId { get; set; }
        public int? CategoryTypeId { get; set; }
        public List<BusinessOutcomeTagResponse> Tags { get; set; }
    }

    public static class BusinessOutcomeCategoryLabelExtensions
    {
        public static BusinessOutcomeCategoryLabelRequest ToRequest(
            this BusinessOutcomeCategoryLabelResponse response)
        {
            return  new BusinessOutcomeCategoryLabelRequest
            {
                Uid = response.Uid,
                Name = response.Name,
                CompanyId = response.CompanyId,
                CreatedAt = response.CreatedAt,
                KanbanMode = response.KanbanMode,
                BusinessOutcomeCardTypeId = (int)response.BusinessOutcomeCardTypeId,
                CategoryTypeId = response.CategoryTypeId,
                Tags = response.Tags.Select(t => new BusinessOutcomeTagRequest
                {
                    Uid = t.Uid,
                    Name = t.Name,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt,
                    DeletedAt = t.DeletedAt,
                    CategoryLabelUid = t.CategoryLabelUid
                }).ToList()
            };
        }
    }
}