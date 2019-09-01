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
public void ConfigureServices(IServiceCollection services)
{
    services.AddHangfire(...);
}

public void Configure(IApplicationBuilder app)
{
    app.UsePrometheusHangfireExporter();
    app.UseMetricServer();
    app.UseHangfireDashboard();
    app.UseHangfireServer();
}
```

## Settings

The following settings are available for this plugin using Hangfire.Prometheus.HangfirePrometheusSettings class:

|     Setting Name      |             Type             |    Default Setting     |                                             Description                                             |
| --------------------- | ---------------------------- | ---------------------- | ----------------------------------------------------------------------------------------------------|
| CollectorRegistry     | Prometheus.CollectorRegistry |Metrics.DefaultRegistry | Prometheus CollectorRegistry to use.                                                                |
| FailScrapeOnException | Boolean                      | true                   | Controls whether to fail the scrape if there is an exception during Hangifre statistics collection. |

An instance of HangfirePrometheusSettings class can be passed to UsePrometheusHangfireExporter() to use settings other than defaults:

```
public void Configure(IApplicationBuilder app)
{
    CollectionRegistry myRegistry = Metrics.NewCustomRegistry();
    app.UsePrometheusHangfireExporter(new HangfirePrometheusSettings { CollectorRegistry = myRegistry });
    app.UseMetricServer(...);
}
```

## Simultaneous Scrapes
Simultaneous scrapes proceed at the same time. Care should be taken when setting the scrape interval period to minimize simultaneous scrapes.

# Multiple Servers
This plugin uses Hangfire job storage to retrieve job statistics. If multiple Hangfire servers are using the same job storage only a single instance should be exporting Hangfire metrics or only a single instance must be scraped. 
