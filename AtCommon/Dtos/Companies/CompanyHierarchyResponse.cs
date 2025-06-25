using System.Collections.Generic;
using System.Linq;
using AtCommon.Api.Enums;
using AtCommon.Utilities;

namespace AtCommon.Dtos.Companies
{
    public class CompanyHierarchyResponse
    {
        public CompanyHierarchyResponse()
        {
            Children = new List<TeamHierarchyResponse>();
        }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<TeamHierarchyResponse> Children { get; set; }
    }

    public class TeamHierarchyResponse
    {
        public TeamHierarchyResponse()
        {
            Children = new List<TeamHierarchyResponse>();
        }
        // From DB
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public string Path { get; set; }
        public bool IsUnassigned { get; set; }
        public string Alias { get; set; }
        public TeamAccessLevel AccessLevel { get; set; }
        public IEnumerable<int> ParentIds { get; set; }
        public List<TeamHierarchyResponse> Children { get; set; }
        public int Depth
        {
            get
            {
                if (Children == null) return 0;
                var path = Children.Select(child => child.Depth).Concat(new[] {0}).Max();
                path++;
                return path;
            }
        }

    }

    public static class HierarchyExtensions
    {
        public static IEnumerable<TeamHierarchyResponse> Descendants(this CompanyHierarchyResponse root)
        {
            var nodes = new Stack<TeamHierarchyResponse>(root.Children);
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in node.Children) nodes.Push(n);
            }
        }

        public static TeamHierarchyResponse GetTeamByName(this CompanyHierarchyResponse root, string teamName)
        {
            return root.Descendants().FirstOrDefault(t => t.Name == teamName)
                .CheckForNull($"<{teamName}> was not found in the response.");
        }
    }
}
