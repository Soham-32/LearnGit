using System.Collections.Generic;

namespace AtCommon.Dtos.Tags
{
    public class MasterTags
    {
        public class Tag
        {
            public string TagName { get; set; }
            public string ParentTagName { get; set; }
        }

        public class Category
        {
            public string CategoryName { get; set; }
            public string Type { get; set; }
            public List<Tag> Tags { get; set; }
        }

        public class AllTags
        {
            public List<Category> Teams { get; set; }
            public List<Category> TeamMembers { get; set; }
            public List<Category> Stakeholders { get; set; }
        }
    }
}
