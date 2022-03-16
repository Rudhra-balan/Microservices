using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Common.Observability.Helper;
using Common.Observability.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Common.Observability
{
    public static class DependencyInjection
    {
        public static void UsePrometheusMetric(this IApplicationBuilder app)
        {
            app.UseMiddleware<MetricMiddleware>();
          //  app.UseMetricsAllMiddleware();
            app.UseMetricServer();
            app.UseHttpMetrics();
        }


        public static void UseMetricsWebTrack(this IHostBuilder hostBuilder)
        {

            hostBuilder.UseMetricsWebTracking();
           
            //hostBuilder.UseMetrics(options =>
            //  {
            //      options.EndpointOptions = endpointOptions =>
            //      {
            //          //endpointOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
            //          //endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            //          endpointOptions.EnvironmentInfoEndpointEnabled = true;
            //          endpointOptions.MetricsEndpointEnabled = true;
            //          endpointOptions.MetricsTextEndpointEnabled = true;

            //      };
            //  });


        }

        public static void AddMetric(this IServiceCollection services)
        {
            var metrics = new MetricsBuilder()
              .Configuration.Configure(
              options =>
              {
                  options.WithGlobalTags((globalTags, info) =>
                  {
                      globalTags.Add("app", info.EntryAssemblyName);
                      globalTags.Add("env", "local-dev");
                  });
              });
            services.AddMetrics(metrics);

            services.AddMetricsTrackingMiddleware();

            services.AddHealthChecks();

            services.AddSingleton<MetricReporter>();

        }


    }
}