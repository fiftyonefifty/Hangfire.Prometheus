using Prometheus;
using System;

namespace Hangfire.Prometheus
{
    public class HangfirePrometheusExporter : IPrometheusExporter
    {
        private readonly IHangfireMonitorService _hangfireMonitorService;
        private readonly CollectorRegistry _collectorRegistry;
        private readonly Gauge _hangfireGauge;

        private readonly string _metricName = "hangfire_job_count";
        private readonly string _metricHelp = "Number of Hangfire jobs";
        private readonly string _stateLabelName = "state";

        private readonly string _failedLabelValue = "failed";
        private readonly string _enqueuedLabelValue = "enqueued";
        private readonly string _scheduledLabelValue = "scheduled";
        private readonly string _processingLabelValue = "processing";
        private readonly string _succeededLabelValue = "succeeded";
        private readonly string _retryLabelValue = "retry";

        public HangfirePrometheusExporter(IHangfireMonitorService hangfireMonitorService, CollectorRegistry collectorRegistry)
        {
            _hangfireMonitorService = hangfireMonitorService ?? throw new ArgumentNullException(nameof(hangfireMonitorService));
            _collectorRegistry = collectorRegistry ?? throw new ArgumentNullException(nameof(collectorRegistry));
            _hangfireGauge = Metrics.WithCustomRegistry(_collectorRegistry).CreateGauge(_metricName, _metricHelp, _stateLabelName);
        }

        public void ExportHangfireStatistics()
        {
            try
            {
                HangfireJobStatistics hangfireJobStatistics = _hangfireMonitorService.GetJobStatistics();
                _hangfireGauge.WithLabels(_failedLabelValue).Set(hangfireJobStatistics.Failed);
                _hangfireGauge.WithLabels(_scheduledLabelValue).Set(hangfireJobStatistics.Scheduled);
                _hangfireGauge.WithLabels(_processingLabelValue).Set(hangfireJobStatistics.Processing);
                _hangfireGauge.WithLabels(_enqueuedLabelValue).Set(hangfireJobStatistics.Enqueued);
                _hangfireGauge.WithLabels(_succeededLabelValue).Set(hangfireJobStatistics.Succeeded);
                _hangfireGauge.WithLabels(_retryLabelValue).Set(hangfireJobStatistics.Retry);
            }
            catch
            {
                //Swallow the exception here since we can't do anything with it.
            }
        }
    }
}
