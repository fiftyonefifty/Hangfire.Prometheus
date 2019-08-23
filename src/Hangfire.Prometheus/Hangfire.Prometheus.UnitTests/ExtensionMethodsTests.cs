using Microsoft.AspNetCore.Builder;
using Moq;
using Prometheus;
using System;
using System.IO;
using Xunit;

namespace Hangfire.Prometheus.UnitTests
{
    public class ExtensionMethodsTests
    {
        [Fact]
        public void UninitializedJobStorage_ThrowsException()
        {
            Mock<IApplicationBuilder> appBuilderMock = new Mock<IApplicationBuilder>();
            appBuilderMock.Setup(x => x.ApplicationServices.GetService(typeof(JobStorage)))
                          .Returns(null);

            Exception ex = Assert.Throws<Exception>(() => appBuilderMock.Object.UsePrometheusHangfireExporter());
            Assert.Equal("Cannot find Hangfire JobStorage class.", ex.Message);
        }

        [Fact]
        public void InitializedJobStorage_DoesNotThrow()
        {
            Mock<IApplicationBuilder> appBuilderMock = new Mock<IApplicationBuilder>();
            appBuilderMock.Setup(x => x.ApplicationServices.GetService(typeof(JobStorage)))
                          .Returns(new Mock<JobStorage>().Object);

            appBuilderMock.Object.UsePrometheusHangfireExporter();
        }
    }
}
