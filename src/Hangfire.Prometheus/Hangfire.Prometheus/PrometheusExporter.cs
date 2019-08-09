using System;
using System.Collections.Generic;
using System.Text;
using Prometheus;

namespace Hangfire.Prometheus
{
    public class PrometheusExporter : IPrometheusExporter
    {
        private readonly IHangfireMonitorService _hangfireMonitorService;

        public PrometheusExporter(IHangfireMonitorService hangfireMonitorService)
        {
            _hangfireMonitorService = hangfireMonitorService;
        }

        public void ExportHangfireStatistics()
        {
            HangfireJobStatistics hangfireJobStatistics = _hangfireMonitorService.GetJobStatistics();
            Gauge hangfireGauge = Metrics.CreateGauge("hangfire_job_count", "Number of Hangfire jobs", "state");
            hangfireGauge.WithLabels("failed").Set(hangfireJobStatistics.Failed);
        }
    }
}
