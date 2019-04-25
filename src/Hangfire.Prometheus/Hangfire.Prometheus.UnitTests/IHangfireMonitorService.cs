namespace Hangfire.Prometheus.UnitTests
{
    internal interface IHangfireMonitorService
    {
        long FailedJobsCount { get; }
    }
}
