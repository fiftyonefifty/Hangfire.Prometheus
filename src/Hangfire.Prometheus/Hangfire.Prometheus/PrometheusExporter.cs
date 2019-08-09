using System;
using System.Collections.Generic;
using System.Text;
using Prometheus;

namespace Hangfire.Prometheus
{
    public class PrometheusExporter : IPrometheusExporter
    {
        private readonly IHangfireMonitorService _hangfireMonitorService;

        private readonly string _metricName = "hangfire_job_count";
        private readonly string _metricHelp = "Number of Hangfire jobs";
        private readonly string _stateLabelName = "state";

        private readonly string _failedLabelValue = "failed";
        private readonly string _enqueuedLabelValue = "enqueued";
        private readonly string _scheduledLabelValue = "scheduled";
        private readonly string _processingLabelValue = "processing";
        private readonly string _succeededLabelValue = "succeeded";
        private readonly string _retryLabelValue = "retry";

        public PrometheusExporter(IHangfireMonitorService hangfireMonitorService)
        {
            _hangfireMonitorService = hangfireMonitorService;
        }

        public void ExportHangfireStatistics()
        {
            HangfireJobStatistics hangfireJobStatistics = _hangfireMonitorService.GetJobStatistics();
            Gauge hangfireGauge = Metrics.CreateGauge(_metricName, _metricHelp, _stateLabelName);
            hangfireGauge.WithLabels(_failedLabelValue).Set(hangfireJobStatistics.Failed);
            hangfireGauge.WithLabels(_scheduledLabelValue).Set(hangfireJobStatistics.Scheduled);
            hangfireGauge.WithLabels(_processingLabelValue).Set(hangfireJobStatistics.Processing);
            hangfireGauge.WithLabels(_enqueuedLabelValue).Set(hangfireJobStatistics.Enqueued);
            hangfireGauge.WithLabels(_succeededLabelValue).Set(hangfireJobStatistics.Succeeded);
            hangfireGauge.WithLabels(_retryLabelValue).Set(hangfireJobStatistics.Retry);
        }
    }
}
