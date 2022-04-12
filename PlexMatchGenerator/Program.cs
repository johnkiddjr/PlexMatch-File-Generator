using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.Options;
using PlexMatchGenerator.Services;
using Serilog;
using Serilog.Enrichers;
using System.CommandLine;

namespace PlexMatchGenerator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await CommandHelper.GenerateRootCommandAndExecuteHandler(args, Run);
        }

        static async Task<int> Run(GeneratorOptions generatorOptions, string[] args)
        {
            var startup = new Startup();

            if (string.IsNullOrEmpty(generatorOptions.LogPath))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override(LogSourceConstants.Microsoft, Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override(LogSourceConstants.System, Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .Enrich.With(new MachineNameEnricher())
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override(LogSourceConstants.Microsoft, Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override(LogSourceConstants.System, Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .Enrich.With(new MachineNameEnricher())
                    .WriteTo.Console()
                    .WriteTo.File(path: $"{generatorOptions.LogPath}{FileConstants.LogFileName}")
                    .CreateLogger();
            }

            Log.Logger.Information(MessageConstants.LoggerAttachedMessage);

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    startup.ConfigureServices(services);
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.GetServiceOrCreateInstance<IGeneratorService>(host.Services);

            return await svc.Run(generatorOptions);
        }
    }
}