using Prometheus;

namespace Hangfire.Prometheus
{
    public class HangfirePrometheusSettings
    {
        public CollectorRegistry CollectorRegistry { get; set; }
        public bool FailScrapeOnException { get; set; }

        public HangfirePrometheusSettings()
        {
            CollectorRegistry = Metrics.DefaultRegistry;
            FailScrapeOnException = true;
        }
    }
}
