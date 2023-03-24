using Microsoft.Extensions.Logging;
//using OpenTelemetry.Metrics;
using Prometheus;
using System.Diagnostics.Metrics;

namespace Metric
{
    public class MetricReporter
    {
        public Counter _requestCounter;
        public Histogram _responseTimeHistogram;
        public const string HttpClientName = "MetricReporterHttpClient";

        public Counter joinedPlayers = Metrics.CreateCounter("JoinedPlayers_total", "all players that have join this game", new CounterConfiguration
        {
            LabelNames = new[] { "player", "positon" }
        });

        public Counter winners = Metrics.CreateCounter("Winners_list_total", "list of winners for the current game", new CounterConfiguration
        {
            LabelNames = new[] { "player", "time", "minormax" }
        });
        public Counter firstPersonRoverMoves = Metrics.CreateCounter("First_Player_Rover_Movement_total", "Count of moves for the first player", new CounterConfiguration
        {
            LabelNames = new[] {"player", "batteryLevel"}
        });
        public Counter firstPersonCopterMoves = Metrics.CreateCounter("First_Player_Copter_Movement_total", "Count of moves for the first player's copter", new CounterConfiguration
        {
            LabelNames = new[] { "player", "batteryLevel" }
        });
        public Counter playersJoinedWhenGameBeforeChangesToPlaying = Metrics.CreateCounter("player_when_game_changes_to_playing_total", "number of player when game offically starts along with their information", new CounterConfiguration
        {
            LabelNames = new[] { "player", "position"}
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
