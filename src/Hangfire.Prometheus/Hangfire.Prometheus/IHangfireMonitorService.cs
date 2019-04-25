namespace Hangfire.Prometheus
{
    public interface IHangfireMonitorService
    {
        long FailedJobsCount { get; }
    }
}