using Prometheus;
using System;

namespace Hangfire.Prometheus
{
    public class HangfirePrometheusSettings
    {
        public CollectorRegistry CollectorRegistry
        {
            get { return _collectorRegistry; }
            set
            {
                _collectorRegistry = value ?? throw new ArgumentNullException();
            }
        }

        private CollectorRegistry _collectorRegistry;

        public bool FailScrapeOnException { get; set; }

        public HangfirePrometheusSettings()
        {
            CollectorRegistry = Metrics.DefaultRegistry;
            FailScrapeOnException = true;
        }
    }
}
