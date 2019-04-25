using Hangfire.Storage;
using Xunit;
using Moq;
using AutoFixture;

namespace Hangfire.Prometheus.UnitTests
{
    public class HangfireMonitorTests
    {
        private Fixture _fixture = new Fixture();
        [Fact]
        public void ShouldGetNumberOfFailedJobs()
        {
            var expectedCount = _fixture.Create<long>();

            var mockMonitorApi = new Mock<IMonitoringApi>();
            mockMonitorApi.Setup(x => x.FailedCount()).Returns(expectedCount);

            IHangfireMonitorService hangfireMonitorService = new HangfireMonitorService(
                mockMonitorApi.Object);

            Assert.Equal(expectedCount, hangfireMonitorService.FailedJobsCount);
        }
    }
}
