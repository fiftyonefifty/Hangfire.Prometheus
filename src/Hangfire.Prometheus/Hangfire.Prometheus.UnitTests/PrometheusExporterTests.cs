using Moq;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AutoFixture;
using System.IO;

namespace Hangfire.Prometheus.UnitTests
{
    public class PrometheusExporterTests
    {
        PrometheusExporter _classUnderTest;

        Mock<IHangfireMonitorService> _mockHangfireMonitor;

        MetricServerMiddleware _prometheusMiddleware;

        IFixture _autoFixture;

        public PrometheusExporterTests()
        {
            _autoFixture = new Fixture();

            _mockHangfireMonitor = new Mock<IHangfireMonitorService>();
            _classUnderTest = new PrometheusExporter(_mockHangfireMonitor.Object);
            _prometheusMiddleware = new MetricServerMiddleware(null, new MetricServerMiddleware.Settings());
        }

        [Fact]
        public void FailedMetricGetsCreated()
        {
            HangfireJobStatistics hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
            _mockHangfireMonitor.Setup(x => x.GetJobStatistics()).Returns(hangfireJobStatistics);

            string metricName = "hangfire_job_count";
            string metricHelp = "Number of Hangfire jobs";
            string stateLabelName = "state";
            string failedLabelValue = "failed";

            string expectedHelpText = $"# HELP {metricName} {metricHelp}\n# TYPE {metricName} gauge";
            string expectedFailedMetricContent = $"{metricName}{{{stateLabelName}=\"{failedLabelValue}\"}} {hangfireJobStatistics.Failed}";

            _classUnderTest.ExportHangfireStatistics();

            string actual = GetPrometheusContent();

            Assert.Contains(expectedHelpText, actual);
            Assert.Contains(expectedFailedMetricContent, actual);

            _mockHangfireMonitor.Verify(x => x.GetJobStatistics(), Times.Once);

        }

        private string GetPrometheusContent()
        {
            FakeHttpContext fakeHttpContext = new FakeHttpContext();
            _prometheusMiddleware.Invoke(fakeHttpContext).Wait();

            string promContent;
            using (MemoryStream myStream = new MemoryStream(((FakeHttpResponse)fakeHttpContext.Response).BodyStream.ToArray()))
            {
                myStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader sw = new StreamReader(myStream))
                {
                    promContent = sw.ReadToEnd();
                }
            }
            return promContent;
        }
    }
}
