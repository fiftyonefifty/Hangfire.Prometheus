using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Hangfire.Prometheus.UnitTests
{
    public class SettingsTests
    {
        [Fact]
        public void SettingsDefaultsTest()
        {
            HangfirePrometheusSettings settings = new HangfirePrometheusSettings();
            Assert.Same(Metrics.DefaultRegistry, settings.CollectorRegistry);
            Assert.True(settings.FailScrapeOnException);
        }

        [Fact]
        public void SettingCustomRegistryPositive()
        {
            HangfirePrometheusSettings settings = new HangfirePrometheusSettings();
            CollectorRegistry collectorRegistry = Metrics.NewCustomRegistry();
            settings.CollectorRegistry = collectorRegistry;
            Assert.Same(collectorRegistry, settings.CollectorRegistry);
        }

        [Fact]
        public void SettingCustomRegistryToNullThrowsException()
        {
            HangfirePrometheusSettings settings = new HangfirePrometheusSettings();
            Assert.Throws<ArgumentNullException>(() => settings.CollectorRegistry = null);
        }

        [Fact]
        public void SettingFailScrapeOnExceptionToNonDefault()
        {
            HangfirePrometheusSettings settings = new HangfirePrometheusSettings();
            settings.FailScrapeOnException = false;
            Assert.False(settings.FailScrapeOnException);
        }
    }
}
