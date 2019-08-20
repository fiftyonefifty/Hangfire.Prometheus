namespace Hangfire.Prometheus
{
    interface IPrometheusExporter
    {
        /// <summary>
        /// Exports current Hangfire job statistics into Prometheus metrics.
        /// </summary>
        void ExportHangfireStatistics();
    }
}
