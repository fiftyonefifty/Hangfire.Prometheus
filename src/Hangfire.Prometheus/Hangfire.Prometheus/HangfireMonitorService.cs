using Hangfire.Storage;

namespace Hangfire.Prometheus
{
    public class HangfireMonitorService : IHangfireMonitorService
    {
        private JobStorage _hangfireJobStorage;

        public HangfireMonitorService(JobStorage hangfireJobStorage)
        {
            _hangfireJobStorage = hangfireJobStorage;
        }
        
        public long FailedJobsCount => _hangfireJobStorage.GetMonitoringApi().GetStatistics().Failed;
    }
}

/*
            IMonitoringApi monitor = JobStorage.Current.GetMonitoringApi();
            StatisticsDto stats = monitor.GetStatistics();
            List<string> ret = new List<string>();
            var fields = stats.GetType().GetProperties();

            foreach (PropertyInfo prop in fields)
            {
                ret.Add($"{prop.Name}: {prop.GetValue(stats)}");
            }

            ret.Add($"Retries: {JobStorage.Current.GetConnection().GetAllItemsFromSet("retries").Count()}");
*/