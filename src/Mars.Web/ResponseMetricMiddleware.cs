using Metric;
using Prometheus;
using Prometheus.HttpClientMetrics;
using System.Diagnostics;

namespace Mars.Web
{
    public class ResponseMetricMiddleware
    {
        private readonly RequestDelegate _request;

        public ResponseMetricMiddleware(RequestDelegate request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public async Task Invoke(HttpContext httpContext, MetricReporter reporter)
        {
            var counter = Metrics.CreateCounter("AllWinners_total", "shows winners", new CounterConfiguration
            {
                LabelNames = new[] {"playerName", "timeCompleted"}
            });
            var path = httpContext.Request.Path.Value;
            if (path == "/metrics")
            {
                await _request.Invoke(httpContext);
                return;
            }
            var sw = Stopwatch.StartNew();

            try
            {
                await _request.Invoke(httpContext);
            }
            finally
            {
                sw.Stop();
                reporter.RegisterRequest();
                reporter.RegisterResponseTime(httpContext.Response.StatusCode, httpContext.Request.Method, sw.Elapsed);
            }
            counter.Labels("Bre", "1.06").Inc();
            

            await _request(httpContext);
        }
    }
}
