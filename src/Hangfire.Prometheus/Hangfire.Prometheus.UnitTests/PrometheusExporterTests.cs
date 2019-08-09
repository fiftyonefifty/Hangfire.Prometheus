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
        private HangfirePrometheusExporter _classUnderTest;

        private Mock<IHangfireMonitorService> _mockHangfireMonitor;

        private IFixture _autoFixture;

        CollectorRegistry _collectorRegistry;

        private readonly string _metricName = "hangfire_job_count";
        private readonly string _metricHelp = "Number of Hangfire jobs";
        private readonly string _stateLabelName = "state";

        private readonly string _failedLabelValue = "failed";
        private readonly string _enqueuedLabelValue = "enqueued";
        private readonly string _scheduledLabelValue = "scheduled";
        private readonly string _processingLabelValue = "processing";
        private readonly string _succeededLabelValue = "succeeded";
        private readonly string _retryLabelValue = "retry";

        public PrometheusExporterTests()
        {
            _autoFixture = new Fixture();

            _mockHangfireMonitor = new Mock<IHangfireMonitorService>();

            _collectorRegistry = Metrics.NewCustomRegistry();
            _classUnderTest = new HangfirePrometheusExporter(_mockHangfireMonitor.Object, _collectorRegistry);
        }

        [Fact]
        public void ConstructorHangfireMonitorNullCheck() => Assert.Throws<ArgumentNullException>(() => new HangfirePrometheusExporter(null, _collectorRegistry));

        [Fact]
        public void ConstructorCollectorRegistryNullCheck() => Assert.Throws<ArgumentNullException>(() => new HangfirePrometheusExporter(_mockHangfireMonitor.Object, null));

        [Fact]
        public void MetricsWithAllStatesGetCreated()
        {
            HangfireJobStatistics hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
            PerformMetricsTest(hangfireJobStatistics);
            _mockHangfireMonitor.Verify(x => x.GetJobStatistics(), Times.Once);

        }

        [Fact]
        public void MetricsWithAllStatesGetUpdatedOnSubsequentCalls()
        {
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                HangfireJobStatistics hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
                PerformMetricsTest(hangfireJobStatistics);
            }

            _mockHangfireMonitor.Verify(x => x.GetJobStatistics(), Times.Exactly(10));
        }

        [Fact]
        public void MetricsShouldNotGetPublishedFirstTimeOnException()
        {
            _mockHangfireMonitor.Setup(x => x.GetJobStatistics()).Throws(new Exception());
            string actual = GetPrometheusContent();

            //The metric description will get published regardless, so we have to test for the labeled metrics.
            Assert.DoesNotContain($"{_metricName}{{{_stateLabelName}=\"", actual);
        }

        [Fact]
        public void MetricsShouldNotGetUpdatedOnException()
        {
            HangfireJobStatistics hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
            PerformMetricsTest(hangfireJobStatistics);

            _mockHangfireMonitor.Setup(x => x.GetJobStatistics()).Throws(new Exception());
            _classUnderTest.ExportHangfireStatistics();
            VerifyPrometheusMetrics(hangfireJobStatistics);

            hangfireJobStatistics = _autoFixture.Create<HangfireJobStatistics>();
            PerformMetricsTest(hangfireJobStatistics);

            _mockHangfireMonitor.Verify(x => x.GetJobStatistics(), Times.Exactly(3));
        }

        private void PerformMetricsTest(HangfireJobStatistics hangfireJobStatistics)
        {
            _mockHangfireMonitor.Setup(x => x.GetJobStatistics()).Returns(hangfireJobStatistics);
            _classUnderTest.ExportHangfireStatistics();
            VerifyPrometheusMetrics(hangfireJobStatistics);
        }

        private void VerifyPrometheusMetrics(HangfireJobStatistics hangfireJobStatistics)
        {
            List<string> expectedStrings = CreateExpectedStrings(hangfireJobStatistics);
            string actual = GetPrometheusContent();

            foreach (string expected in expectedStrings)
            {
                Assert.Contains(expected, actual);
            }
        }

        private List<string> CreateExpectedStrings(HangfireJobStatistics hangfireJobStatistics)
        {
            List<string> expectedStrings = new List<string>();
            expectedStrings.Add($"# HELP {_metricName} {_metricHelp}\n# TYPE {_metricName} gauge");
            expectedStrings.Add(GetMetricString(_failedLabelValue, hangfireJobStatistics.Failed));
            expectedStrings.Add(GetMetricString(_enqueuedLabelValue, hangfireJobStatistics.Enqueued));
            expectedStrings.Add(GetMetricString(_scheduledLabelValue, hangfireJobStatistics.Scheduled));
            expectedStrings.Add(GetMetricString(_processingLabelValue, hangfireJobStatistics.Processing));
            expectedStrings.Add(GetMetricString(_succeededLabelValue, hangfireJobStatistics.Succeeded));
            expectedStrings.Add(GetMetricString(_retryLabelValue, hangfireJobStatistics.Retry));
            return expectedStrings;
        }

        private string GetMetricString(string labelValue, double metricValue)
        {
            return $"{_metricName}{{{_stateLabelName}=\"{labelValue}\"}} {metricValue}";
        }

        private string GetPrometheusContent()
        {
            string content;
            using (MemoryStream myStream = new MemoryStream())
            {
                _collectorRegistry.CollectAndExportAsTextAsync(myStream).Wait();
                myStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(myStream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }
    }
}
