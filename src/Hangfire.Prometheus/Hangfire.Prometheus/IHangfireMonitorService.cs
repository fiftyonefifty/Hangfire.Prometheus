namespace Hangfire.Prometheus
{
    public interface IHangfireMonitorService
    {
        /// <summary>
        /// Obtains all job statistics from Hangfire.
        /// </summary>
        /// <returns>Instance of HangfireJobStatistics class containing the current stats for Hangfire jobs.</returns>
        HangfireJobStatistics GetJobStatistics();
    }
}