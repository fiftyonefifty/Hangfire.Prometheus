using Moq;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.Prometheus.UnitTests
{
    public class PrometheusExporterTests
    {
        PrometheusExporter _classUnderTest;

        Mock<IHangfireMonitorService> _mockHangfireMonitor;

        MetricServerMiddleware _prometheusMiddleware;

        public PrometheusExporterTests()
        {
            _mockHangfireMonitor = new Mock<IHangfireMonitorService>();
            _classUnderTest = new PrometheusExporter(_mockHangfireMonitor.Object);
            _prometheusMiddleware = new MetricServerMiddleware(null, new MetricServerMiddleware.Settings());
        }
    }
}
