namespace PxWeb.Code.Api2.Cache
{
    public class DefaultCacheClearer
    {
        public static DateTime NextClearTime { get; set; } = DateTime.Now;

        public static void SetNextClearTime(DateTime nextClearTime)
        {
            if (nextClearTime < DateTime.Now)
            {
                nextClearTime = nextClearTime.AddDays(1);
            }
            NextClearTime = nextClearTime;
        }

        public static bool CacheIsCoherent()
        {
            if (DateTime.Now >= NextClearTime)
            {
                NextClearTime = NextClearTime.AddDays(1);
                return false;
            }
            return true;
        }
    }
}
