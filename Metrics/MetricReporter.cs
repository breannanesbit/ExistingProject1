using Microsoft.Extensions.Logging;
//using OpenTelemetry.Metrics;
using Prometheus;
using System.Diagnostics.Metrics;

namespace Metric
{
    public class MetricReporter
    {
        private readonly ILogger<MetricReporter> _logger;
        public Counter _requestCounter;
        public Histogram _responseTimeHistogram;
        public const string HttpClientName = "MetricReporterHttpClient";

        public Counter joinedPlayers = Metrics.CreateCounter("JoinedPlayers_total", "all players that have join this game", new CounterConfiguration
        {
            LabelNames = new[] { "player", "gameid" }
        });

        public Counter winners = Metrics.CreateCounter("Winners_list_total", "list of winners for the current game", new CounterConfiguration
        {
            LabelNames = new[] { "player", "gameid", "time" }
        });

        public MetricReporter()
        {
        
            _requestCounter =
                Metrics.CreateCounter("Winners_total", "All winners");

            _responseTimeHistogram = Metrics.CreateHistogram("request_duration_seconds",
                "The duration in seconds between the response to a request.", new Prometheus.HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                    LabelNames = new[] { "status_code", "method" }
                });
        }

        public void RegisterRequest()
        {
            _requestCounter.Inc();
        }

        public void RegisterResponseTime(int statusCode, string method, TimeSpan elapsed)
        {
            _responseTimeHistogram.Labels(statusCode.ToString(), method).Observe(elapsed.TotalSeconds);
        }
    }
}
