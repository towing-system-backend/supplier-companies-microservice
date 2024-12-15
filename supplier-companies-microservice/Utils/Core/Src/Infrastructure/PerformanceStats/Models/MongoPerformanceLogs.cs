namespace Application.Core
{
    public class MongoPerformanceLogs(string serviceExecuted, long latencyTime)
    {
        public string ServiceExecuted = serviceExecuted;
        public long LatencyTime = latencyTime;
        public DateTime CreatedAt = DateTime.Now;
    }
}
