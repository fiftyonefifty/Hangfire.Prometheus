using System;
using System.Collections.Generic;
using System.Text;

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
            throw new NotImplementedException();
        }
    }
}
