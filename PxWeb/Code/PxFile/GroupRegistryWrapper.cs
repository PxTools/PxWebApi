using PCAxis.Paxiom;

namespace PxWeb.Code.PxFile
{
    /// <summary>
    /// Wrapper class for GroupRegistry to make it easier to mock in tests
    /// </summary>
    public class GroupRegistryWrapper
    {
        public virtual List<GroupingInfo> GetDefaultGroupings(string Domain)
        {
            return GroupRegistry.GetRegistry().GetDefaultGroupings(Domain);
        }

        public virtual Grouping GetGrouping(GroupingInfo groupingInfo)
        {
            return GroupRegistry.GetRegistry().GetGrouping(groupingInfo);
        }

        public virtual bool IsLoaded { get { return GroupRegistry.GetRegistry().GroupingsLoaded; } }
    }
}
