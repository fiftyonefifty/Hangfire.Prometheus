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
        private PrometheusExporter _classUnderTest;

        private Mock<IHangfireMonitorService> _mockHangfireMonitor;

        private MetricServerMiddleware _prometheusMiddleware;

        private IFixture _autoFixture;

        private readonly string _metricName = "hangfire_job_count";
        private readonly string _metricHelp = "Number of Hangfire jobs";
        private readonly string _stateLabelName = "state";

        public PrometheusExporterTests()
        {
            _autoFixture = new Fixture();

            _mockHangfireMonitor = new Mock<IHangfireMonitorService>();
            _classUnderTest = new PrometheusExporter(_mockHangfireMonitor.Object);
            _prometheusMiddleware = new MetricServerMiddleware(null, new MetricServerMiddleware.Settings());
        }

        [Fact]
        public void MetricsWithAllStatesGetCreated()
        {
            HangfireJobStatistics hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
            _mockHangfireMonitor.Setup(x => x.GetJobStatistics()).Returns(hangfireJobStatistics);

            string failedLabelValue = "failed";
            string enqueuedLabelValue = "enqueued";
            string scheduledLabelValue = "scheduled";
            string processingLabelValue = "processing";
            string succeededLabelValue = "succeeded";
            string retryLabelValue = "retry";

            List<string> expectedStrings = new List<string>();

            expectedStrings.Add($"# HELP {_metricName} {_metricHelp}\n# TYPE {_metricName} gauge");
            expectedStrings.Add(GetMetricString(failedLabelValue, hangfireJobStatistics.Failed));
            expectedStrings.Add(GetMetricString(enqueuedLabelValue, hangfireJobStatistics.Enqueued));
            expectedStrings.Add(GetMetricString(scheduledLabelValue, hangfireJobStatistics.Scheduled));
            expectedStrings.Add(GetMetricString(processingLabelValue, hangfireJobStatistics.Processing));
            expectedStrings.Add(GetMetricString(succeededLabelValue, hangfireJobStatistics.Succeeded));
            expectedStrings.Add(GetMetricString(retryLabelValue, hangfireJobStatistics.Retry));

            _classUnderTest.ExportHangfireStatistics();
            string actual = GetPrometheusContent();

            foreach (string expected in expectedStrings)
            {
                Assert.Contains(expected, actual);
            }

            _mockHangfireMonitor.Verify(x => x.GetJobStatistics(), Times.Once);

        }

        private string GetMetricString(string labelValue, double metricValue)
        {
            return $"{_metricName}{{{_stateLabelName}=\"{labelValue}\"}} {metricValue}";
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
