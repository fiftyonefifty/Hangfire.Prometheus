namespace Hangfire.Prometheus
{
    public class HangfireJobStatistics
    {
        /// <summary>
        /// Represents number of jobs in enqueued state.
        /// </summary>
        public long Enqueued { get; set; }

        /// <summary>
        /// Represents number of jobs in schedule state.
        /// </summary>
        public long Scheduled { get; set; }

        /// <summary>
        /// Represents number of jobs in processing state.
        /// </summary>
        public long Processing { get; set; }

        /// <summary>
        /// Represents number of jobs in succeeded state.
        /// </summary>
        public long Succeeded { get; set; }

        /// <summary>
        /// Represents number of jobs in failed state.
        /// </summary>
        public long Failed { get; set; }

        /// <summary>
        /// Represents number of jobs in retry state.
        /// </summary>
        public long Retry { get; set; }
    }
}
