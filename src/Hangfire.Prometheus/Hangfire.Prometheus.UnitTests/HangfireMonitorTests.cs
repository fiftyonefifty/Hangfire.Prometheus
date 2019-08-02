using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Xunit;
using Moq;
using AutoFixture;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace Hangfire.Prometheus.UnitTests
{
    public class HangfireMonitorTests
    {
        private static readonly string retryKey = "retries";

        private Fixture _fixture = new Fixture();
        private StatisticsDto _expectedStats;
        private HashSet<string> _expectedRetrySet;
        Mock<JobStorage> _mockStorage;
        IHangfireMonitorService _hangfireMonitorService;

        public HangfireMonitorTests()
        {
            _expectedStats = _fixture.Create<StatisticsDto>();
            _expectedRetrySet = new HashSet<string>();
            _expectedRetrySet.AddMany(() => _fixture.Create<string>(), new Random().Next(100));

            Mock<IStorageConnection> storageConnection = new Mock<IStorageConnection>();
            storageConnection.Setup(x => x.GetAllItemsFromSet(retryKey)).Returns(_expectedRetrySet);

            Mock<IMonitoringApi> mockMonitoringApi = new Mock<IMonitoringApi>();
            mockMonitoringApi.Setup(x => x.GetStatistics()).Returns(_expectedStats);

            _mockStorage = new Mock<JobStorage>();
            _mockStorage.Setup(x => x.GetConnection()).Returns(storageConnection.Object);
            _mockStorage.Setup(x => x.GetMonitoringApi()).Returns(mockMonitoringApi.Object);

            _hangfireMonitorService = new HangfireMonitorService(_mockStorage.Object);
        }

        [Fact]
        public void ShouldGetNumberOfJobs()
        {
            HangfireJobStatistics actual = _hangfireMonitorService.GetJobStatistics();

            Assert.Equal(_expectedStats.Failed, actual.Failed);
            Assert.Equal(_expectedStats.Enqueued, actual.Enqueued);
            Assert.Equal(_expectedStats.Scheduled, actual.Scheduled);
            Assert.Equal(_expectedStats.Processing, actual.Processing);
            Assert.Equal(_expectedStats.Succeeded, actual.Succeeded);
            Assert.Equal(_expectedRetrySet.Count, actual.Retry);
        }
    }
}
