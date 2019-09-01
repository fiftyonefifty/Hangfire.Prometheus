using Prometheus;
using System;

namespace Hangfire.Prometheus
{
    public class HangfirePrometheusSettings
    {
        /// <summary>
        /// Prometheus collector registry to use. Defaults to Metrics.DefaultRegistry.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if CollectorRegistry is set to null.</exception>
        public CollectorRegistry CollectorRegistry
        {
            get { return _collectorRegistry; }
            set
            {
                _collectorRegistry = value ?? throw new ArgumentNullException();
            }
        }
        private CollectorRegistry _collectorRegistry;

        /// <summary>
        /// Determines behavior when there is an error during Hangfire statistics collection. If set to true,
        /// ScrapeException is thrown to indicate that scrape should be aborted. If set to false, any error during
        /// collection is ignored and metric values will not be updated.
        /// </summary>
        public bool FailScrapeOnException { get; set; }

        public HangfirePrometheusSettings()
        {
            CollectorRegistry = Metrics.DefaultRegistry;
            FailScrapeOnException = true;
        }
    }
}
