using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Xunit;
using Moq;
using AutoFixture;
using System.Collections.Generic;
using System;

namespace Hangfire.Prometheus.UnitTests
{
    public class HangfireMonitorTests
    {
        private static readonly string retryKey = "retries";

        private Fixture _fixture = new Fixture();
        private StatisticsDto _expectedStats;
        private HashSet<string> _expectedRetrySet;
        Mock<JobStorage> _mockStorage;

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
        }

        [Fact]
        public void ShouldGetNumberOfFailedJobs()
        {
            long expectedCount = _expectedStats.Failed;

            IHangfireMonitorService hangfireMonitorService = new HangfireMonitorService(_mockStorage.Object);

            Assert.Equal(expectedCount, hangfireMonitorService.FailedJobsCount);
        }
    }
}
