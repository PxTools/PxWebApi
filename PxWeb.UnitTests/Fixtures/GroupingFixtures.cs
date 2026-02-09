namespace PxWeb.UnitTests.Fixtures
{
    internal static class GroupingFixtures
    {
        internal readonly static GroupingInfo MartialStatusInfo = new("in-memory-martial-status")
        {
            Name = "Marital status",
            GroupPres = GroupingIncludesType.AggregatedValues
        };

        internal readonly static GroupingInfo SexInfo = new("in-memory-sex")
        {
            Name = "Sex",
            GroupPres = GroupingIncludesType.AggregatedValues
        };

        internal readonly static Grouping GroupMaritalStatus;
        internal readonly static Grouping GroupSex;

        static GroupingFixtures()
        {
            // Marital status
            GroupMaritalStatus = new Grouping();
            GroupMaritalStatus.Name = "In memmory";
            GroupMaritalStatus.Map = null;
            GroupMaritalStatus.Groups.Add(new Group()
            {
                GroupCode = "S",
                Name = "Small",
                ChildCodes = [new() { Code = "OG" }]
            });

            GroupMaritalStatus.Groups.Add(new Group()
            {
                GroupCode = "L",
                Name = "Large",
                ChildCodes = [new() { Code = "G" }, new() { Code = "ÄNKL" }, new() { Code = "SK" }]
            });

            GroupMaritalStatus.Groups.Add(new Group()
            {
                GroupCode = "T",
                Name = "Total",
                ChildCodes = [new() { Code = "total" }]
            });

            // Sex
            GroupSex = new Grouping();
            GroupSex.Name = "In memmory";
            GroupSex.Map = null;
            GroupSex.Groups.Add(new Group()
            {
                GroupCode = "T2",
                Name = "Grouped Total",
                ChildCodes = [new() { Code = "1" }, new() { Code = "2" }]
            });
        }

    }
}
