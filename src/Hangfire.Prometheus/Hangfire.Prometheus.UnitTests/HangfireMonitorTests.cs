using AutoFixture;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

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
        Mock<IStorageConnection> _storageConnection;

        public HangfireMonitorTests()
        {
            _expectedStats = _fixture.Create<StatisticsDto>();
            _expectedRetrySet = new HashSet<string>();
            _expectedRetrySet.AddMany(() => _fixture.Create<string>(), new Random().Next(100));

            _storageConnection = new Mock<IStorageConnection>();
            _storageConnection.Setup(x => x.GetAllItemsFromSet(retryKey)).Returns(_expectedRetrySet);
            _storageConnection.Setup(x => x.Dispose());

            Mock<IMonitoringApi> mockMonitoringApi = new Mock<IMonitoringApi>();
            mockMonitoringApi.Setup(x => x.GetStatistics()).Returns(_expectedStats);

            _mockStorage = new Mock<JobStorage>();
            _mockStorage.Setup(x => x.GetConnection()).Returns(_storageConnection.Object);
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

        [Fact]
        public void ShouldDisposeOfStorageConnection()
        {
            _hangfireMonitorService.GetJobStatistics();
            _storageConnection.Verify(x => x.Dispose(), Times.AtLeastOnce);

        }
    }
}
