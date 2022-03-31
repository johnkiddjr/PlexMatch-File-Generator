﻿// check for a paramter of the plex token, prompt if not present
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.Services;
using Serilog;
using Serilog.Enrichers;

namespace PlexMatchGenerator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var generatorOptions = ArgumentHelper.ProcessCommandLineArguments(args.ToList());

            var startup = new Startup();

            if (string.IsNullOrEmpty(generatorOptions.LogPath))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .Enrich.With(new MachineNameEnricher())
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .Enrich.With(new MachineNameEnricher())
                    .WriteTo.Console()
                    .WriteTo.File(path: $"{generatorOptions.LogPath}plexmatch.log")
                    .CreateLogger();
            }

            Log.Logger.Information("Logger attached. Startup complete. Running file generator...");

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