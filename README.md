# Hangfire.Prometheus
Simple plugin for .NET Core applications to export Hangfire stats to Prometheus.

# Description
The plugin uses the Hangfire JobStorage class to export metric "hangfire_job_count" using "state" label to indicate jobs in various states. The metrics are updated before every scrape. The states exported are:

* Failed
* Enqueued
* Scheduled
* Processing
* Succeeded
* Retry

# Usage
Hangfire.Prometheus plugin is initialized in Configure() using UseHangfirePrometheusExporter() method. Hangfire job storage must already be initialized.

```
Example to follow
```

# Multiple Servers
This plugin uses Hangfire job storage to retrieve job statistics. If multiple Hangfire servers are using the same job storage only a single instance should be exporting Hangfire metrics or only a single instance must be scraped. 

# Behavior
The following is the behavior of this plugin:

* Uses default metrics registry (Metrics.DefaultRegistry).
* Uses job storage registered with Hangfire.
* Exception during metrics update *_does not block_* scrapes.
* Simultaneous scrapes proceed at the same time.
