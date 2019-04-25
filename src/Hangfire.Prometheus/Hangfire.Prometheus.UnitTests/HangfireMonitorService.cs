using Hangfire.Storage;

namespace Hangfire.Prometheus.UnitTests
{
    internal class HangfireMonitorService : IHangfireMonitorService
    {
        private IMonitoringApi _hangfireMonitoringApi;

        public HangfireMonitorService(IMonitoringApi hangfireMonitoringApi)
        {
            _hangfireMonitoringApi = hangfireMonitoringApi;
        }

        public long FailedJobsCount => _hangfireMonitoringApi.FailedCount();
    }
}
