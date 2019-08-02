namespace Hangfire.Prometheus
{
    public interface IHangfireMonitorService
    {
        HangfireJobStatistics GetJobStatistics();
    }
}