using Serilog;

namespace ECommerce.API.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
        {
            host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

            return host;
        }
    }
}
